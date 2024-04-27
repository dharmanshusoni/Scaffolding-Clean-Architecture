using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scaffolding.Core.Business_Interface.ServiceQuery;
using Scaffolding.Core.Business_Interface;
using Scaffolding.Core.Model;
using Microsoft.Extensions.Caching.Memory;
using Scaffolding.Core.Entities;
using System.Threading.Tasks;
using System.Threading;

namespace Scaffolding.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        // In-Memory Caching
        private const string messageListCacheKey = "messageList";
        private readonly IMemoryCache _cache;

        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        private readonly IMessageServiceQuery messageServiceQuery;
        private readonly IMessageService messageService;
        
        public MessageController(IMessageServiceQuery messageServiceQuery, IMessageService messageService, IMemoryCache cache)
        {
            this.messageServiceQuery = messageServiceQuery;
            this.messageService = messageService;
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // In-Memory Caching
            if (_cache.TryGetValue(messageListCacheKey, out IEnumerable<Message> messageList))
            {
                #region list found in cache
                return Ok(messageList);
                #endregion
            }

            var messages = this.messageServiceQuery.GetAll();

            // In-Memory Caching
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(1024);
            _cache.Set(messageListCacheKey, messages, cacheEntryOptions);

            return Ok(messages);
        }

        [HttpGet("get-all-concurrent-access")]
        public async Task<IActionResult> GetAllConcurrentAccess()
        {
            // In-Memory Caching
            if (_cache.TryGetValue(messageListCacheKey, out IEnumerable<Message> messageList))
            {
                #region list found in cache
                return Ok(messageList);
                #endregion
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(messageListCacheKey, out messageList))
                    {
                        #region list found in cache
                        return Ok(messageList);
                        #endregion
                    }
                    else
                    {
                        #region list not found in cache. Fetching from database
                        var messages = this.messageServiceQuery.GetAll();
                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                                .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                                .SetPriority(CacheItemPriority.Normal)
                                .SetSize(1024);
                        _cache.Set(messageListCacheKey, messages, cacheEntryOptions);
                        #endregion

                        return Ok(messages);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        [HttpGet("received-messages/{userId}")]
        public IActionResult GetUserReceivedMessages(string userId)
        {
            var messages = this.messageServiceQuery.GetReceivedMessages(userId);
            return Ok(messages);
        }
   
        [HttpPost()]
        public async Task<IActionResult> DeleteMessage([FromBody] MessageDeleteModel messageDeleteModel)
        {
            var message = await this.messageService.DeleteMessage(messageDeleteModel);
            return Ok(message);
        }
    }
}