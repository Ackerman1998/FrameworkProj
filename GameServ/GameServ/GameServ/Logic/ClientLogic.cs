using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServ.Utils;
namespace GameServ.Logic
{
    class ClientLogic
    {
        /// <summary>
        /// Login Method
        /// </summary>
        /// <param name="bufferbyte"></param>
        public void Login (MessageNode node) {
            Bufferbyte bufferbyte = node.bufferbyte;
            try
            {
                int count = bufferbyte.ReadInt();
                string password = bufferbyte.ReadString();
                Console.WriteLine("Login count =" + count + ",password=" + password);
                SendSuccess(node);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e);
            }
            finally {
                bufferbyte.Clear();
            }
        }
        /// <summary>
        /// Send success to client
        /// </summary>
        /// <param name="node"></param>
        public void SendSuccess(MessageNode node) {
            Bufferbyte bufferbyte = new Bufferbyte();
            bufferbyte.WriteString("LoginSuccess");
            bufferbyte.WriteString("Conguraduation ! Login Success !");
            bufferbyte.SendMessage(node.client);
        }
        /// <summary>
        /// Register Method
        /// </summary>
        /// <param name="bufferbyte"></param>
        public void Register(Bufferbyte bufferbyte) {

        }
    }
}
