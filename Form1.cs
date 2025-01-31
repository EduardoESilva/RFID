﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFID
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            byte[] arrBuffer = new byte[4096];
            String strPort = "COM10";
            if (RFID.CFComApi.CFCom_OpenDevice(strPort, 115200))
            {
                this.SetText("Conectado\r\n");
                if (RFID.CFComApi.CFCom_GetDeviceSystemInfo(0xFF, arrBuffer) == false)
                {
                    this.SetText("Erro de Conexão\r\n");
                    //RFID.CFComApi.CFCom_CloseDevice();
                    //return;
                }
            }
            else
            {
                this.SetText("Falha\r\n");
                return;
            }

            string str = "", str1 = "";
            str = String.Format("SoftVer:{0:D}.{0:D}\r\n", arrBuffer[0] >> 4, arrBuffer[0] & 0x0F);
            this.SetText(str);
            str = String.Format("HardVer:{0:D}.{0:D}\r\n", arrBuffer[1] >> 4, arrBuffer[1] & 0x0F);
            this.SetText(str);
            str = "SN:";
            for (int i = 0; i < 7; i++)
            {
                str1 = String.Format("{0:X2}", arrBuffer[2 + i]);
                str = str + str1;
            }
            str = str + "\r\n";
            this.SetText(str);
            
            //Potencia de Sinal

            byte bParamAddr = 0;
            byte bValue = 0;

            bParamAddr = 0x05;

            bValue = (byte)Convert.ToInt16("17");

            if (RFID.CFComApi.CFCom_SetDeviceOneParam(0xFF, bParamAddr, bValue) == false)
            {
                this.SetText("Falha");
                return;
            }
            this.SetText("Sucesso\r\n");

            //Modo Ativo

            this.SetText("Modo Ativo\r\n");
            RFID.CFComApi.CFCom_ClearTagBuf();
            timer1.Interval = 100;
            timer1.Enabled = true;

            //Leitura Continua

            if (!RFID.CFComApi.CFCom_OpenDevice(strPort, 115200))
            {
                ushort iNum = 0;
                ushort iTotalLen = 0;
                this.SetText("Modo Comandos\r\n");
                if (RFID.CFComApi.CFCom_InventoryG2(0xFF, arrBuffer, out iTotalLen, out iNum) == false)
                {
                    this.SetText("Falha\r\n");
                    return;
                }
                int iTagLength = 0;
                int iTagNumber = 0;
                iTagLength = iTotalLen;
                iTagNumber = iNum;
                if (iTagNumber == 0) return;
                int iIndex = 0;
                int iLength = 0;
                byte bPackLength = 0;
                int i = 0;

                for (iIndex = 0; iIndex < iTagNumber; iIndex++)
                {
                    bPackLength = arrBuffer[iLength];
                    string str2 = "";
                    string str4 = "";
                    str4 = arrBuffer[1 + iLength + 0].ToString("X2");
                    str2 = str2 + "Tipo:" + str4 + " ";  //Tag Type

                    str4 = arrBuffer[1 + iLength + 1].ToString("X2");
                    str2 = str2 + "Ant:" + str4 + " Tag:";  //Ant

                    string str3 = "";
                    for (i = 2; i < bPackLength - 1; i++)
                    {
                        str4 = arrBuffer[1 + iLength + i].ToString("X2");
                        str3 = str3 + str4 + "";
                    }
                    str2 = str2 + str3;
                    str4 = arrBuffer[1 + iLength + i].ToString("X2");
                    str2 = str2 + " RSSI:" + str4 + "\r\n";  //RSSI
                    iLength = iLength + bPackLength + 1;
                    this.SetText(str2);
                }
            }
            else
            {
                timer2.Interval = 300;
                timer2.Enabled = !timer2.Enabled;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            byte[] arrBuffer = new byte[4096];
            int iNum = 0;
            int iTotalLen = 0;
            byte bRet = 0;

            bRet = RFID.CFComApi.CFCom_GetTagBuf(arrBuffer, out iTotalLen, out iNum);
            if (bRet == 1)
            {
                this.SetText("Erro de Conexão");
                return; //DevOut
            }
            else if (bRet == 0) return; //No Connect
            int iTagLength = 0;
            int iTagNumber = 0;
            iTagLength = iTotalLen;
            iTagNumber = iNum;
            if (iTagNumber == 0) return;
            int iIndex = 0;
            int iLength = 0;
            byte bPackLength = 0;
            int i = 0;

            for (iIndex = 0; iIndex < iTagNumber; iIndex++)
            {
                bPackLength = arrBuffer[iLength];
                string str2 = "";
                string str1 = "";
                str1 = arrBuffer[1 + iLength + 0].ToString("X2");
                str2 = str2 + "Tipo:" + str1 + " ";  //Tag Type

                str1 = arrBuffer[1 + iLength + 1].ToString("X2");
                str2 = str2 + "Ant:" + str1 + " Tag:";  //Ant

                string str3 = "";
                for (i = 2; i < bPackLength - 1; i++)
                {
                    str1 = arrBuffer[1 + iLength + i].ToString("X2");
                    str3 = str3 + str1 + "";
                }
                str2 = str2 + str3;
                str1 = arrBuffer[1 + iLength + i].ToString("X2");
                str2 = str2 + " RSSI:" + str1 + "\r\n";  //RSSI
                iLength = iLength + bPackLength + 1;
                this.SetText(str2);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            byte[] arrBuffer = new byte[4096];
            ushort iNum = 0;
            ushort iTotalLen = 0;
            this.SetText("Modo Comandos\r\n");
            if (RFID.CFComApi.CFCom_InventoryG2(0xFF, arrBuffer, out iTotalLen, out iNum) == false)
            {
                this.SetText("Falha\r\n");
                return;
            }
            int iTagLength = 0;
            int iTagNumber = 0;
            iTagLength = iTotalLen;
            iTagNumber = iNum;
            if (iTagNumber == 0) return;
            int iIndex = 0;
            int iLength = 0;
            byte bPackLength = 0;
            int i = 0;

            for (iIndex = 0; iIndex < iTagNumber; iIndex++)
            {
                bPackLength = arrBuffer[iLength];
                string str2 = "";
                string str1 = "";
                str1 = arrBuffer[1 + iLength + 0].ToString("X2");
                str2 = str2 + "Tipo:" + str1 + " ";  //Tag Type

                str1 = arrBuffer[1 + iLength + 1].ToString("X2");
                str2 = str2 + "Ant:" + str1 + " Tag:";  //Ant

                string str3 = "";
                for (i = 2; i < bPackLength - 1; i++)
                {
                    str1 = arrBuffer[1 + iLength + i].ToString("X2");
                    str3 = str3 + str1 + "";
                }
                str2 = str2 + str3;
                str1 = arrBuffer[1 + iLength + i].ToString("X2");
                str2 = str2 + " RSSI:" + str1 + "\r\n";  //RSSI
                iLength = iLength + bPackLength + 1;
                this.SetText(str2);
            }
        }

        delegate void SetTextCallback(string text);
        private void SetText(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.Text = this.textBox1.Text + text;
                this.textBox1.SelectionStart = this.textBox1.Text.Length;
                this.textBox1.ScrollToCaret();
            }
        }
    }
}
