using MAVTOOL.mavlink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//***************************************************//
//**阿木社区编写(AMOV AUTO),玩也要玩的专业！
//** www.amovauto.com
//***本代码仅供参考，如有疑问请上社区论坛或者QQ群询问//
namespace MAVTOOL
{
    class UpdateComUiThread
    {
        private MAVLinkInterface comPortMP;

      
        //声明一个readmavlink类型的对象。该对象代表了返回值为空.  
        public event readmavlink readmavlinkEvent;
        public UpdateComUiThread(MAVLinkInterface comPort)
        {
            comPortMP = comPort;

           
        }

        public void readmavlinkdelegate()
        {
            while (comPortMP.BaseStream.IsOpen)
            {
               // foreach (var MAV in comPortMP.MAVlist.GetMAVStates())
                {
                    try
                    {
                        comPortMP.MAV.cs.UpdateCurrentSettings(null, false, comPortMP, comPortMP.MAV);
                    }
                    catch (Exception ex)
                    {
                        ;//没有写，异常反馈！
                    }
                    if (readmavlinkEvent != null)
                    {
                        readmavlinkEvent.Invoke(comPortMP.MAV, comPortMP.IP);
                    }
                 

                }
                Thread.Sleep(5);
            }
        }

   }
}    


    
 


