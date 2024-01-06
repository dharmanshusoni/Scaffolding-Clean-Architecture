using Microsoft.EntityFrameworkCore;
using Scaffolding.Core.Business_Interface;
using Scaffolding.Core.Entities;
using Scaffolding.Core.Enums;
using Scaffolding.Core.Model;
using Scaffolding.Core.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaffolding.Business
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork unitOfWork;
        public MessageService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public void Add(Message message)
        {
            unitOfWork.Repository<Message>().Add(message);
            unitOfWork.SaveChanges();
        }
        async Task<Message> IMessageService.DeleteMessage(MessageDeleteModel messageDeleteModel)
        {
            // var message = messageDeleteModel.Message;
            var messageRepo = unitOfWork.Repository<Message>();
            var message = await messageRepo.Get().Where(x => x.Id == messageDeleteModel.Message.Id).FirstOrDefaultAsync();
            if (messageDeleteModel.DeleteType == DeleteTypeEnum.DeleteForEveryone.ToString())
            {
                message.IsReceiverDeleted = true;
                message.IsSenderDeleted = true;
            }
            else
            {
                message.IsReceiverDeleted = message.IsReceiverDeleted || message.Receiver == messageDeleteModel.DeletedUserId;
                message.IsSenderDeleted = message.IsSenderDeleted || message.Sender == messageDeleteModel.DeletedUserId;
            }
            messageRepo.Update(message);
            await unitOfWork.SaveChangesAsync();
            return message;
        }
    }
}
