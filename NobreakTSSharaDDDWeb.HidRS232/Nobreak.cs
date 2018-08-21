using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using NobreakTSSharaDDDWeb.HidRS232;

namespace NobreakTSSharaDDDweb.HidRS232
{
    //[Serializable()]
    //public class Nobreak : MarshalByRefObject, IUpsDevice
    public class Nobreak : IUpsDevice
    {
        private static bool MyDeviceDetected;
        private static DeviceManagement MyDeviceManagement;
        private static string RespostaHID;
        private static int HIDHandle;
        private static string MyDevicePathName;
        private static Hid MyHID;
        private static int ReadHandle;
        private static int WriteHandle;
        private static Hid.OutputReport myOutputReport;
        private static Hid.InputReport myInputReport;

        static string vid;
        static string pid;
        private DateTime UpdatedAt;
        private DateTime Q1UpdatedAt;
        private bool lastStatusConectado;

        public event EventHandler<NobreakEventArgs> RedeOkEvent,
                                                    BateriaBaixaEvent,
                                                    ComunicacaoOkEvent,
                                                    FalhaComunicacaoEvent,
                                                    FalhaDeRedeEvent,
                                                    BateriaOkEvent,
                                                    AnormalidadeEvent;
        //public event FalhaDeRedeEventHandler FalhaDeRedeEvent;
        //public event BateriaOkEventHandler BateriaOkEvent;
        ////public event DesligamentoSistema;
        //public event HibernacaoSistema;
        //public event RetornoSistema;
        //public event TesteSolicitado;
        //public event RetornoTeste;
        //public event ModoBypass;
        //public event AnormalidadeEventHandler AnormalidadeEvent;

        public Nobreak()
        {
            vid = "0483";
            pid = "0035";
            MyHID = new Hid();
            MyDeviceManagement = new DeviceManagement();
            myOutputReport = new Hid.OutputReport();
            myInputReport = new Hid.InputReport();
        }

        public Nobreak(string _vid, string _pid)
        {
            vid = _vid;
            pid = _pid;
            MyHID = new Hid();
            MyDeviceManagement = new DeviceManagement();
            myOutputReport = new Hid.OutputReport();
            myInputReport = new Hid.InputReport();
        }

        //TODO Criar exception para nobreak nao conectado
        public UpsData UpsInquiry()
        {
            try
            {
                string[] resultQ1 = WriteAndReadFromUPS("Q1\r").Replace('(', ' ').Replace("\r", " ").Replace(".", ",").Split(new char[] { ' ' });
                string[] resultF = WriteAndReadFromUPS("F\r").Replace('(', ' ').Replace("\r", " ").Replace(".", ",").Split(new char[] { ' ' }); ;

                UpsData info = new UpsData();
                if (resultQ1.Count<string>() == 10)
                {
                    char[] chArray = resultQ1[8].ToCharArray();

                    int _OMaximumCurrent; Int32.TryParse(resultQ1[4], out _OMaximumCurrent);
                    double _Temperature; Double.TryParse(resultQ1[7], out _Temperature);

                    info = new UpsData
                    {
                        Success = true,
                        TensaoEntrada = Convert.ToDouble(resultQ1[1]),
                        TensaoEntrada2 = Convert.ToDouble(resultQ1[2]),
                        TensaoSaida = Convert.ToDouble(resultQ1[3]),
                        OMaximumCurrent = _OMaximumCurrent,
                        Frequencia = Convert.ToDouble(resultQ1[5]),
                        TensaoBateria = Convert.ToDouble(resultQ1[6]),
                        Temperatura = _Temperature,
                        UpsStatus = resultQ1[8],
                        UtilityFailImmediate = CharToBool(chArray[0]),
                        BateriaBaixa = CharToBool(chArray[1]),
                        StatusBypassBoostBuckActive = CharToBool(chArray[2]),
                        FalhaUps = CharToBool(chArray[3]),
                        UpsStandBy = CharToBool(chArray[4]),
                        TesteEmProgresso = CharToBool(chArray[5]),
                        DesligamentoAtivo = CharToBool(chArray[6]),
                        BeepLigado = CharToBool(chArray[7]),
                        Carga = 0
                    };


                    string fdados = WriteAndReadFromUPS("F\r");
                    NobreakInfo.TensaoSaidaNominal = Conversion.Val(Strings.Mid(fdados, 2, 3)) + Conversion.Val(Strings.Mid(fdados, 6, 1)) / 10.0;
                    NobreakInfo.CargaNominal = checked((long)Math.Round(Conversion.Val(Strings.Mid(fdados, 8, 3))));
                    NobreakInfo.FrequenciaNominal = Conversion.Val(Strings.Mid(fdados, 24, 2)) + Conversion.Val(Strings.Mid(fdados, 27, 1)) / 10.0;
                    if (StringType.StrCmp(Strings.Mid(fdados, 14, 1), ".", false) == 0)
                    {
                        NobreakInfo.TensaoBateriaNominal = Conversion.Val(Strings.Mid(fdados, 12, 2)) + Conversion.Val(Strings.Mid(fdados, 15, 2)) / 100.0;
                    }
                    if (StringType.StrCmp(Strings.Mid(fdados, 15, 1), ".", false) != 0)
                    {
                        //return null;
                    }
                    NobreakInfo.TensaoBateriaNominal = Conversion.Val(Strings.Mid(fdados, 12, 3)) + Conversion.Val(Strings.Mid(fdados, 16, 1)) / 10.0;

                    if (info.TensaoBateria != -1.0)
                    {
                        if (NobreakInfo.TensaoBateriaNominal != 0.0 & info.TensaoBateria != 0.0)
                        {
                            int num = !(info.TensaoBateria > 1.0 & info.TensaoBateria < 3.0 & NobreakInfo.TensaoBateriaNominal > 100.0) ? 1 : 96;
                            NobreakInfo.PorcentagemBateria = checked((int)Math.Round(unchecked(100.0 * (info.TensaoBateria * (double)num - NobreakInfo.TensaoBateriaNominal * 0.833) / (NobreakInfo.TensaoBateriaNominal * 1.0833 * 0.23))));
                            if (NobreakInfo.PorcentagemBateria > 100)
                                NobreakInfo.PorcentagemBateria = 100;
                            if (NobreakInfo.PorcentagemBateria < 0)
                                NobreakInfo.PorcentagemBateria = 0;
                        }
                        else
                            NobreakInfo.PorcentagemBateria = 0;
                    }
                    else
                        NobreakInfo.PorcentagemBateria = -1;

                    info.PorcentagemBateria = NobreakInfo.PorcentagemBateria;

                    return info;
                }

                return info;
            }
            catch (NobreakNotConnectedException ex)
            {
                return new UpsData() { Success = false };
            }

        }

        public InformationData InformationInquiry()
        {
            var info = new InformationData();

            string[] result = ExecutaI().Replace('(', ' ').Replace("\r", " ").Replace(".", ",").Split(new char[] { ' ' });
            if (result.Count<string>() == 5)
            {
                info.Fabricante = result[1];
                info.Modelo = result[2];
                info.Versao = result[3];
                info.Success = true;
            }

            return info;
        }

        public void TurnOnOffBeep()
        {
            try
            {
                WriteAndReadFromUPS("Q\r");
            }
            catch (NobreakNotConnectedException ex)
            {
                Debug.WriteLine("Nobreak nao conectado para alternar beep");
            }
        }

        public void TestForMinutes(int minutes)
        {
            try
            {

                if ((minutes > 0x63) || (minutes < 1))
                    throw new ArgumentException("Tempo inválido. Range permitido para minutos: 01-99");

                string strMinutes = "";

                if (minutes < 10)
                {
                    strMinutes = "0" + minutes.ToString();
                }
                else
                {
                    strMinutes = minutes.ToString();
                }

                WriteAndReadFromUPS($"T{strMinutes}\r");
            }
            catch (NobreakNotConnectedException)
            {
                Debug.WriteLine("Nobreak não conectado para executar o teste!"); //TODO: Codigo repetido para veriricar conexao do nobreak
                //throw new Exception("Nobreak não conectado");
            }
        }

        public void TestFor10Seconds()
        {
            try
            {
                WriteAndReadFromUPS("T\r");
            }
            catch (NobreakNotConnectedException)
            {
                Debug.WriteLine("Nobreak não conectado para executar o teste!");
            }
        }

        public void TestUntilBatteryLow()
        {
            try
            {
                WriteAndReadFromUPS("TL\r");
            }
            catch (NobreakNotConnectedException)
            {
                Debug.WriteLine("Nobreak não conectado para executar o teste!");
            }
        }

        public void CancelTestCommand()
        {
            try
            {
                WriteAndReadFromUPS("CT\r");
            }
            catch (NobreakNotConnectedException)
            {
                Debug.WriteLine("Nobreak não conectado para executar o teste!");
            }
        }

        /// <summary>
        /// Informação dos status de energia
        /// </summary>
        /// <returns></returns>
        public StatusInfo Status()
        {
            var status = new StatusInfo();
            var upsData = UpsInquiry();

            if (Conectado())
            {
                status.Conectado = true;
                NobreakEventManager.FireEventComunicacao(OnComunicacaoOkEvent, this, new NobreakEventArgs() { UpSdata = upsData });

                if (upsData.StatusBypassBoostBuckActive)
                {
                    status.Rede = StatusRede.ModoByPass;
                    //OnModoByPass(this, new EventArgs());
                }
                if (NobreakInfo.StatusFalhaUps)
                {
                    status.Rede = StatusRede.Anormal;
                    NobreakEventManager.FireEventComunicacao(OnAnormalidadeEvent, this, new NobreakEventArgs() { UpSdata = upsData });
                }
                if (!upsData.StatusBypassBoostBuckActive & !upsData.FalhaUps)
                {
                    if (upsData.UtilityFailImmediate | upsData.TesteEmProgresso)
                    {
                        status.Rede = StatusRede.OperandoEmBateria;
                        //OnFalhaDeRedeEvent(this, new EventArgs());
                        NobreakEventManager.FireEventRede(OnFalhaDeRedeEvent, this, new NobreakEventArgs());
                    }
                    else
                    {
                        status.Rede = StatusRede.OperandoEmRede;
                        //OnRedeOkEvent(this, new NobreakEventArgs());
                        NobreakEventManager.FireEventRede(RedeOkEvent, this, new NobreakEventArgs() { UpSdata = upsData });

                    }
                }
                if (upsData.BateriaBaixa)
                {
                    status.Bateria = StatusBateria.Baixa;
                    //OnBateriaBaixaEvent(new EventArgs());
                    NobreakEventManager.FireEventBateria(OnBateriaBaixaEvent, this, new NobreakEventArgs() { UpSdata = upsData });

                }
                else
                {
                    status.Bateria = StatusBateria.Ok;
                    NobreakEventManager.FireEventBateria(OnBateriaOk, this, new NobreakEventArgs() { UpSdata = upsData });
                }

                status.Conectado = true;
            }
            else
            {

                status.Conectado = false;
                NobreakEventManager.FireEventComunicacao(OnFalhaComunicaoEvent, this, new NobreakEventArgs());
                status.Rede = StatusRede.Desconhecido;
                status.Bateria = StatusBateria.Desconhecido;
            }

            lastStatusConectado = status.Conectado;
            return status;
        }

        //TODO: Refatorar método
        public bool Conectado()
        {
            if (UpdatedAt == null || (DateTime.Now - UpdatedAt).Seconds > 2)
            {
                string[] devicePathName = new string[128];
                try
                {
                    Guid empty = Guid.Empty;
                    MyDeviceDetected = false;
                    FileIOApiDeclarations.SECURITY_ATTRIBUTES lpSecurityAttributes = new FileIOApiDeclarations.SECURITY_ATTRIBUTES();
                    lpSecurityAttributes.lpSecurityDescriptor = 0;
                    lpSecurityAttributes.bInheritHandle = -1;
                    lpSecurityAttributes.nLength = Strings.Len(lpSecurityAttributes);
                    //short num1 = checked((short)Math.Round(Conversion.Val("&h" + vid)));
                    //short num2 = checked((short)Math.Round(Conversion.Val("&h" + pid)));

                    short num1 = checked((short)Math.Round(Conversion.Val("&h0483")));
                    short num2 = checked((short)Math.Round(Conversion.Val("&h0035")));

                    HidApiDeclarations.HidD_GetHidGuid(ref empty);
                    empty.ToString();
                    if (MyDeviceManagement.FindDeviceFromGuid(empty, ref devicePathName))
                    {
                        int index = 0;
                        do
                        {
                            HIDHandle = FileIOApiDeclarations.CreateFile(devicePathName[index], 0, 3, ref lpSecurityAttributes, 3, 0, 0);
                            if (HIDHandle != -1)
                            {
                                MyHID.DeviceAttributes.Size = Marshal.SizeOf((object)MyHID.DeviceAttributes);
                                bool flag;
                                if (HidApiDeclarations.HidD_GetAttributes(HIDHandle, ref MyHID.DeviceAttributes))
                                {
                                    if ((int)MyHID.DeviceAttributes.VendorId == (int)num1 & (int)MyHID.DeviceAttributes.ProductId == (int)num2)
                                    {
                                        MyDeviceDetected = true;
                                        MyDevicePathName = devicePathName[index];
                                        NobreakInfo.StatusComunicacaoOk = true;
                                    }
                                    else
                                    {
                                        MyDeviceDetected = false;
                                        flag = FileIOApiDeclarations.CloseHandle(HIDHandle);
                                    }
                                }
                                else
                                {
                                    MyDeviceDetected = false;
                                    flag = FileIOApiDeclarations.CloseHandle(HIDHandle);
                                }
                            }
                            checked { ++index; }
                        }
                        while (!(MyDeviceDetected | index == checked(Information.UBound((Array)devicePathName, 1) + 1)));
                    }

                    lastStatusConectado = MyDeviceDetected;
                    UpdatedAt = DateTime.Now;
                    return MyDeviceDetected;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERRO!!! " + ex.Message);
                    //throw new NobreakNotConnectedException("Não foi possível conectar-se ao Nobreak");

                }

            }

            return lastStatusConectado;

        }

        //Information Command
        public string ExecutaI()
        {
            string result = "";
            try
            {
                result = WriteAndReadFromUPS("I\r");
            }
            catch (NobreakNotConnectedException)
            {
                Debug.WriteLine("Nobreak nao conectado para executar o comando 'I'");
            }


            return result;
        }

        private static bool CharToBool(char Bit)
        {
            return (Bit == '1');
        }

        private string WriteAndReadFromUPS(string Comando, bool NaoEsperaResposta = false)
        {
            if (!Conectado())
                throw new NobreakNotConnectedException();

            FileIOApiDeclarations.SECURITY_ATTRIBUTES lpSecurityAttributes = new FileIOApiDeclarations.SECURITY_ATTRIBUTES();
            MyHID.Capabilities = MyHID.GetDeviceCapabilities(HIDHandle);
            WriteHandle = FileIOApiDeclarations.CreateFile(MyDevicePathName, 1073741824, 3, ref lpSecurityAttributes, 3, 0, 0);
            ReadHandle = FileIOApiDeclarations.CreateFile(MyDevicePathName, int.MinValue, 3, ref lpSecurityAttributes, 3, 1073741824, 0);

            string str = "Str";
            try
            {
                bool flag = false;
                if (ReadHandle != -1 & WriteHandle != -1)
                {
                    if ((int)MyHID.Capabilities.OutputReportByteLength > 0)
                    {
                        byte[] reportBuffer = new byte[checked((int)MyHID.Capabilities.OutputReportByteLength - 1 + 1)];
                        reportBuffer[0] = (byte)0;
                        if (Information.UBound((Array)reportBuffer, 1) > 1)
                        {
                            reportBuffer[1] = checked((byte)Strings.Len(Comando));
                            int num1 = 1;
                            int num2 = Strings.Len(Comando);
                            int Start = num1;
                            while (Start <= num2)
                            {
                                reportBuffer[checked(Start + 1)] = checked((byte)Strings.Asc(Strings.Mid(Comando, Start, 1)));
                                checked { ++Start; }
                            }
                        }
                        flag = myOutputReport.Write(reportBuffer, WriteHandle);
                        if (!flag)
                            MyDeviceDetected = false;
                    }
                    string Expression1 = "Expression 1";
                    if (!NaoEsperaResposta & flag)
                    {
                        Expression1 = "";
                        int num1 = 0;
                        bool success;
                        do
                        {
                            success = false;
                            if ((int)MyHID.Capabilities.InputReportByteLength > 0)
                            {
                                byte[] readBuffer = new byte[checked((int)MyHID.Capabilities.InputReportByteLength - 1 + 1)];
                                myInputReport.Read(ReadHandle, HIDHandle, WriteHandle, ref MyDeviceDetected, ref readBuffer, ref success);
                                if (success)
                                {
                                    string Expression2 = Expression1;
                                    int num2 = 1;
                                    int num3 = (int)readBuffer[1];
                                    int num4 = num2;
                                    while (num4 <= num3)
                                    {
                                        if ((int)readBuffer[checked(num4 + 1)] != 0)
                                            Expression1 += StringType.FromChar(Strings.Chr((int)readBuffer[checked(num4 + 1)]));
                                        checked { ++num4; }
                                    }
                                    if (Strings.Len(Expression1) == Strings.Len(Expression2) | Strings.Len(Expression1) == 47 & StringType.StrCmp(Comando, "Q1\r", false) == 0 | Strings.Len(Expression1) == 22 & StringType.StrCmp(Comando, "F\r", false) == 0)
                                        checked { ++num1; }
                                    else
                                        num1 = 0;
                                }
                                else
                                    MyDeviceDetected = false;
                            }
                        }
                        while (success & num1 < 1);
                    }
                    str = Expression1;
                }
            }

            catch (NobreakNotConnectedException)
            {
                Debug.WriteLine("Nobreak nao conectado para executar COMANDO");
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                //frmMonitor.HandleException(Name, ex);
                ProjectData.ClearProjectError();
                Console.WriteLine("ERRO> " + ex.Message);
            }


            return str;
        }

        private void ParseQ1(string q1Result)
        {
            string q1Dados = q1Result; // modVariaveis.Q1Dados;
            NobreakInfo.TensaoEntrada = !(StringType.StrCmp(Strings.Mid(q1Dados, 2, 3), "@@@", false) != 0 & StringType.StrCmp(Strings.Mid(q1Dados, 6, 1), "@", false) != 0) ? -1.0 : Conversion.Val(Strings.Mid(q1Dados, 2, 3)) + Conversion.Val(Strings.Mid(q1Dados, 6, 1)) / 10.0;
            NobreakInfo.TensaoEntrada2 = !(StringType.StrCmp(Strings.Mid(q1Dados, 8, 3), "@@@", false) != 0 & StringType.StrCmp(Strings.Mid(q1Dados, 12, 1), "@", false) != 0) ? -1.0 : Conversion.Val(Strings.Mid(q1Dados, 8, 3)) + Conversion.Val(Strings.Mid(q1Dados, 12, 1)) / 10.0;
            NobreakInfo.TensaoSaida = !(StringType.StrCmp(Strings.Mid(q1Dados, 14, 3), "@@@", false) != 0 & StringType.StrCmp(Strings.Mid(q1Dados, 18, 1), "@", false) != 0) ? -1.0 : Conversion.Val(Strings.Mid(q1Dados, 14, 3)) + Conversion.Val(Strings.Mid(q1Dados, 18, 1)) / 10.0;
            NobreakInfo.Carga = StringType.StrCmp(Strings.Mid(q1Dados, 20, 3), "@@@", false) == 0 ? -1L : checked((long)Math.Round(Conversion.Val(Strings.Mid(q1Dados, 20, 3))));
            NobreakInfo.Frequencia = !(StringType.StrCmp(Strings.Mid(q1Dados, 24, 2), "@@", false) != 0 & StringType.StrCmp(Strings.Mid(StringType.FromInteger(27), 1), "@", false) != 0) ? -1.0 : Conversion.Val(Strings.Mid(q1Dados, 24, 2)) + Conversion.Val(Strings.Mid(q1Dados, 27, 1)) / 10.0;
            NobreakInfo.TensaoBateria = !(StringType.StrCmp(Strings.Mid(q1Dados, 29, 2), "@@", false) != 0 & StringType.StrCmp(Strings.Mid(q1Dados, 32, 1), "@", false) != 0) ? -1.0 : Conversion.Val(Strings.Mid(q1Dados, 29, 2)) + Conversion.Val(Strings.Mid(q1Dados, 32, 1)) / 10.0;
            NobreakInfo.Temperatura = !(StringType.StrCmp(Strings.Mid(q1Dados, 34, 2), "@@", false) != 0 & StringType.StrCmp(Strings.Mid(q1Dados, 37, 1), "@", false) != 0) ? -1.0 : Conversion.Val(Strings.Mid(q1Dados, 34, 2)) + Conversion.Val(Strings.Mid(q1Dados, 37, 1)) / 10.0;
            NobreakInfo.StatusUtilityFail = BooleanType.FromObject(Interaction.IIf(StringType.StrCmp(Strings.Mid(q1Dados, 39, 1), "1", false) == 0, (object)true, (object)false));
            NobreakInfo.StatusBateriaBaixa = BooleanType.FromObject(Interaction.IIf(StringType.StrCmp(Strings.Mid(q1Dados, 40, 1), "1", false) == 0, (object)true, (object)false));
            NobreakInfo.StatusBypassBoostBuckActive = BooleanType.FromObject(Interaction.IIf(StringType.StrCmp(Strings.Mid(q1Dados, 41, 1), "1", false) == 0, (object)true, (object)false));
            NobreakInfo.StatusFalhaUps = BooleanType.FromObject(Interaction.IIf(StringType.StrCmp(Strings.Mid(q1Dados, 42, 1), "1", false) == 0, (object)true, (object)false));
            NobreakInfo.StatusUpsStandBy = BooleanType.FromObject(Interaction.IIf(StringType.StrCmp(Strings.Mid(q1Dados, 43, 1), "1", false) == 0, (object)true, (object)false));
            NobreakInfo.StatusTesteEmProgresso = BooleanType.FromObject(Interaction.IIf(StringType.StrCmp(Strings.Mid(q1Dados, 44, 1), "1", false) == 0, (object)true, (object)false));
            NobreakInfo.StatusShutdownAtivo = BooleanType.FromObject(Interaction.IIf(StringType.StrCmp(Strings.Mid(q1Dados, 45, 1), "1", false) == 0, (object)true, (object)false));
            NobreakInfo.StatusBeepLigado = BooleanType.FromObject(Interaction.IIf(StringType.StrCmp(Strings.Mid(q1Dados, 46, 1), "1", false) == 0, (object)true, (object)false));

        }

        private void ParseF()
        {
            string fdados = NobreakInfo.FDados;
            NobreakInfo.TensaoSaidaNominal = Conversion.Val(Strings.Mid(fdados, 2, 3)) + Conversion.Val(Strings.Mid(fdados, 6, 1)) / 10.0;
            NobreakInfo.CargaNominal = checked((long)Math.Round(Conversion.Val(Strings.Mid(fdados, 8, 3))));
            NobreakInfo.FrequenciaNominal = Conversion.Val(Strings.Mid(fdados, 24, 2)) + Conversion.Val(Strings.Mid(fdados, 27, 1)) / 10.0;
            if (StringType.StrCmp(Strings.Mid(fdados, 14, 1), ".", false) == 0)
                NobreakInfo.TensaoBateriaNominal = Conversion.Val(Strings.Mid(fdados, 12, 2)) + Conversion.Val(Strings.Mid(fdados, 15, 2)) / 100.0;
            if (StringType.StrCmp(Strings.Mid(fdados, 15, 1), ".", false) != 0)
                return;
            NobreakInfo.TensaoBateriaNominal = Conversion.Val(Strings.Mid(fdados, 12, 3)) + Conversion.Val(Strings.Mid(fdados, 16, 1)) / 10.0;
        }

        private void AtualizaValores()
        {
            if (NobreakInfo.TensaoBateria != -1.0)
            {
                if (NobreakInfo.TensaoBateriaNominal != 0.0 & NobreakInfo.TensaoBateria != 0.0)
                {
                    int num = !(NobreakInfo.TensaoBateria > 1.0 & NobreakInfo.TensaoBateria < 3.0 & NobreakInfo.TensaoBateriaNominal > 100.0) ? 1 : 96;
                    NobreakInfo.PorcentagemBateria = checked((int)Math.Round(unchecked(100.0 * (NobreakInfo.TensaoBateria * (double)num - NobreakInfo.TensaoBateriaNominal * 0.833) / (NobreakInfo.TensaoBateriaNominal * 1.0833 * 0.23))));
                    if (NobreakInfo.PorcentagemBateria > 100)
                        NobreakInfo.PorcentagemBateria = 100;
                    if (NobreakInfo.PorcentagemBateria < 0)
                        NobreakInfo.PorcentagemBateria = 0;
                }
                else
                    NobreakInfo.PorcentagemBateria = 0;
            }
            else
                NobreakInfo.PorcentagemBateria = -1;

        }

        #region Eventos
        public virtual void OnBateriaBaixaEvent(object sender, NobreakEventArgs e)
        {
            if (BateriaBaixaEvent != null)
                BateriaBaixaEvent(this, e);
        }

        public virtual void OnComunicacaoOkEvent(object sender, NobreakEventArgs e)
        {
            if (ComunicacaoOkEvent != null)
                ComunicacaoOkEvent(sender, e);
        }

        public virtual void OnFalhaDeRedeEvent(object sender, NobreakEventArgs e)
        {
            FalhaDeRedeEvent?.Invoke(this, e);
        }

        public virtual void OnRedeOkEvent(object sender, NobreakEventArgs e)
        {
            RedeOkEvent?.Invoke(this, e);
        }

        public virtual void OnFalhaComunicaoEvent(object sender, NobreakEventArgs e)
        {
            if (FalhaComunicacaoEvent != null)
                FalhaComunicacaoEvent(this, e);
        }

        public virtual void OnAnormalidadeEvent(object sender, NobreakEventArgs e)
        {
            if (AnormalidadeEvent != null)
                AnormalidadeEvent(sender, e);
        }

        private void OnBateriaOk(object sender, NobreakEventArgs e)
        {
            if (BateriaOkEvent != null)
                BateriaOkEvent(sender, e);
        }
        #endregion 

    }


    public delegate void BateriaBaixaEventHandler(object sender, NobreakEventArgs e);
    public delegate void ComunicacaoOkEventHandler(object sender, NobreakEventArgs e);
    public delegate void FalhaDeRedeEventHandler(object sender, NobreakEventArgs e);
    public delegate void RedeOkEventHandler(object sender, NobreakEventArgs e);
    public delegate void FalhaComunicacaoEventHandler(object sender, NobreakEventArgs e);
    public delegate void AnormalidadeEventHandler(object sender, NobreakEventArgs e);
    public delegate void BateriaOkEventHandler(object sender, NobreakEventArgs e);




}
