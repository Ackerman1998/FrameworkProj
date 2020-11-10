using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Framework { 
     class MessageHandler 
    {
        public void LoginSuccess(MessageNode node) {
            node.bufferbyte.ReadString().Log();

        }
    }
}