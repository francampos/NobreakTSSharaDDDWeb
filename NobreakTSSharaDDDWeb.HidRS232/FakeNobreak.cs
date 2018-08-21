using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NobreakTSSharaDDDWeb.HidRS232;

namespace NobreakTSSharaDDDweb.HidRS232
{
    //[Serializable()]
    //public class Nobreak : MarshalByRefObject, IUpsDevice
    public class FakeNobreak : IUpsDevice
    {
        private static bool MyDeviceDetected;
        private static DeviceManagement MyDeviceManagement;
        private static Hid.OutputReport myOutputReport;
        private static Hid.InputReport myInputReport;
        private static Random random = new Random();
        double probalidadeDeEstaConectado = 0.98;


        static string vid;
        static string pid;
        private DateTime UpdatedAt;
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

        public FakeNobreak()
        {
            vid = "0483";
            pid = "0035";
            MyDeviceManagement = new DeviceManagement();
            myOutputReport = new Hid.OutputReport();
            myInputReport = new Hid.InputReport();
        }

        public FakeNobreak(string _vid, string _pid)
        {
            vid = _vid;
            pid = _pid;
            MyDeviceManagement = new DeviceManagement();
            myOutputReport = new Hid.OutputReport();
            myInputReport = new Hid.InputReport();
        }

        //TODO Criar exception para nobreak nao conectado
        public UpsData UpsInquiry()
        {
            try
            {
                UpsData info = new UpsData();

                info = new UpsData
                {
                    Success = true,
                    TensaoEntrada = RandomNumber(119.0, 127.0),
                    TensaoEntrada2 = 127.0,
                    TensaoSaida = RandomNumber(119.0, 124.0),
                    OMaximumCurrent = 50,
                    Frequencia = RandomNumber(58.0, 61.9),
                    TensaoBateria = RandomNumber(10, 12),
                    Temperatura = RandomNumber(15, 28),
                    UpsStatus = "UPS STATUS",
                    UtilityFailImmediate = false,
                    BateriaBaixa = false,
                    StatusBypassBoostBuckActive = ProbabilityBoolean(0.1), //false,
                    FalhaUps = ProbabilityBoolean(0.2),
                    UpsStandBy = false,
                    TesteEmProgresso = false,
                    DesligamentoAtivo = false,
                    BeepLigado = true
                };
                info.PorcentagemBateria = RandomNumber(30, 100);
                
                return info;
            }
            catch (NobreakNotConnectedException ex)
            {
                return new UpsData() { Success = false };
            }

        }

        public InformationData InformationInquiry()
        {
            var info = new InformationData()
            {
                Fabricante = "TS SHARA (Fake)",
                Modelo = "UPS 700",
                Versao = "2.32",
                Success = true
            };



            return info;
        }

        public void TurnOnOffBeep()
        {
            Debug.WriteLine("Turn Beep");
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

                Debug.WriteLine("teste por 10 minutos executando...");
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
                Debug.WriteLine("teste por 10 segundos executando...");
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
                Debug.WriteLine("teste ate bateria baixar executando...");
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
                Debug.WriteLine("cancelando teste executando...");
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
        public StatusInfo Status()//TODO Mudar nome do método!!!
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
                        NobreakEventManager.FireEventRede(OnFalhaDeRedeEvent, this, new NobreakEventArgs() { UpSdata = upsData });
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
                MyDeviceDetected = ProbabilityBoolean(98.0) ;
                NobreakInfo.StatusComunicacaoOk = true;
                lastStatusConectado = MyDeviceDetected;
                UpdatedAt = DateTime.Now;
                return MyDeviceDetected;

            }

            return lastStatusConectado;

        }

        private bool ProbabilityBoolean(double probability)
        {
            return random.NextDouble() < (probability / 100);
        }

        private static bool CharToBool(char Bit)
        {
            return (Bit == '1');
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

        public double RandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return Math.Round(random.NextDouble() * (maximum - minimum) + minimum, 2);
        }

    }
}
