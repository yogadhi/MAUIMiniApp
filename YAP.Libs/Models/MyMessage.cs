using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAP.Libs.Models
{
    public class MessageContainer
    {
        public string Key { get; set; }
        public object CustomObject { get; set; }
    }

    public class MyMessage : ValueChangedMessage<MessageContainer>
    {
        public MyMessage(MessageContainer value) : base(value)
        {

        }
    }
}
