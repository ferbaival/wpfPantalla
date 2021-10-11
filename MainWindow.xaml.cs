using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
namespace WpfPantalla
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }
        public zkemkeeper.CZKEM axCZKEM1 = new zkemkeeper.CZKEM();


        /******************************************************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first. 
        * This part is for demonstrating the communication with your device.The main commnication ways of Iface series are "TCP/IP","Serial Port" 
        * The communication way which you can use duing to the model of the device you have.
        * ****************************************************************************************************************************************/
        #region Communicacion
        private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.
        private string _ruta;
        public void Conectar()
        {
            Cursor = Cursors.Wait;
            bIsConnected = axCZKEM1.Connect_Net("192.168.10.21", 4370);
            if (axCZKEM1.RegEvent(iMachineNumber, 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            {
                this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
                this.axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
                                
            }
            Cursor = Cursors.Hand;
            
        }
        public void MostrarDatosdePrueba(int idCliente)
        {
            Cursor = Cursors.Wait;
            //bIsConnected = axCZKEM1.Connect_Net("192.168.10.21", 4370);
            //if (axCZKEM1.RegEvent(iMachineNumber, 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            //{
            //    this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
               //this.axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);

            //}
            try
            {

                var bc = new BrushConverter();
                borde.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#E600B82A");
                
                GymEntities conn = new GymEntities();
                List<Cliente> numQuery =
                (from num in conn.Clientes
                 where (num.id) == idCliente
                 select num ).ToList();
                foreach (Cliente num in numQuery)
                {

                    txtNombre.Text = num.nombres;
                    txtApp.Text = num.apellidos;
                    txtFecha.Text = "Finaliza: " + num.fechafin.ToShortDateString();
                    txtDiasrestantes.Text = CalcularFechafin(num.fechafin);

                    //<<<<<<<<<<<<<<<<<<<<<CARGAR LA FOTO>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                    byte[] img = num.foto;
                    BitmapImage imagen;
                    if (img == null)
                    {
                        Obtener_Ruta();
                        //PONE LA FOTO SEGUN EL SEXO Y SI NO TIENE SEXO PONE LA FOTO DE HOMBRE
                        if (num.sexo == "F")
                        {
                            _ruta = _ruta + "\\userFemale.png";
                            imagen = new BitmapImage(new Uri(_ruta));

                            imgRostro.Source = imagen;
                        }
                        if (num.sexo == "M")
                        {
                            _ruta = _ruta + "\\userMale.png";
                            imagen = new BitmapImage(new Uri(_ruta));

                            imgRostro.Source = imagen;
                        }
                        if (num.sexo == null)
                        {
                            _ruta = _ruta + "\\userMale.png";
                            imagen = new BitmapImage(new Uri(_ruta));

                            imgRostro.Source = imagen;
                        }

                    }
                    else
                    {
                        MostrarFoto(img);

                    }
                    if (int.Parse(txtDiasrestantes.Text) <= 5)
                    {
                        //Storyboard sb = this.FindResource("borde") as Storyboard;
                        //sb.Begin();
                        if (int.Parse(txtDiasrestantes.Text) == 1)
                        {
                            tituloQuedan.Text = "TE QUEDA";
                            tituloDias.Text = "DIA";
                        }
                        borde.Background = System.Windows.Media.Brushes.Red;
                        
                        borde.Opacity = 0.9;
                    }
                    else
                    {


                        //Storyboard sb = this.FindResource("borde") as Storyboard;
                        //sb.Resume();

                    }

                }
                //txtCodigo.Text="Ci: "+ sEnrollNumber.ToString();
                Storyboard sb = this.FindResource("Dias") as Storyboard;
                sb.Begin();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            Cursor = Cursors.Hand;

        }//////////////////////////FIN DE DATOS DE PRUEBA

        #endregion
        #region Eventos en Tiempo Real
        private void axCZKEM1_OnVerify(int iUserID)
        {
            //var bc = new BrushConverter();
            //borde.Background= (Brush)bc.ConvertFrom("#E600B82A");           
            //Storyboard sb = this.FindResource("Dias") as Storyboard;
            //sb.Begin();

        }
        private void axCZKEM1_OnAttTransactionEx(string sEnrollNumber, int iIsInValid, int iAttState, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode)
        {
            try
            {
          
            var bc = new BrushConverter();
            borde.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#E600B82A");           

            GymEntities conn = new GymEntities();
            List<Cliente> numQuery =
            (from num in conn.Clientes
            where (num.codigo) == sEnrollNumber
            select num).ToList();
            foreach (Cliente num in numQuery)
            {

                txtNombre.Text = num.nombres;
                txtApp.Text = num.apellidos;
                txtFecha.Text ="Finaliza: "+ num.fechafin.ToShortDateString();
                txtDiasrestantes.Text = CalcularFechafin(num.fechafin);
                
                //<<<<<<<<<<<<<<<<<<<<<CARGAR LA FOTO>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                byte[] img = num.foto;
                BitmapImage imagen;
                if (img == null)
                {
                    Obtener_Ruta();
                    //PONE LA FOTO SEGUN EL SEXO Y SI NO TIENE SEXO PONE LA FOTO DE HOMBRE
                    if (num.sexo=="F")
                    {
                         _ruta = _ruta + "\\userFemale.png";
                     imagen = new BitmapImage(new Uri(_ruta));

                     imgRostro.Source = imagen;   
                    }
                    if (num.sexo == "F")
                    {
                        _ruta = _ruta + "\\userMale.png";
                        imagen = new BitmapImage(new Uri(_ruta));

                        imgRostro.Source = imagen;
                    }
                    if (num.sexo==null)
                    {
                       _ruta = _ruta + "\\userMale.png";
                      imagen = new BitmapImage(new Uri(_ruta));

                     imgRostro.Source = imagen;    
                    }
                     
                }
                else
                {
                    MostrarFoto(img);

                }
                if (int.Parse( txtDiasrestantes.Text) <= 5)
                {
                    //Storyboard sb = this.FindResource("borde") as Storyboard;
                    //sb.Begin();
                    if (int.Parse( txtDiasrestantes.Text) == 1)
                    {
                        tituloQuedan.Text = "TE QUEDA";
                        tituloDias.Text = "DIA";
                    }
					borde.Background= System.Windows.Media.Brushes.Red;
                }
				else
				{
                    

                    //Storyboard sb = this.FindResource("borde") as Storyboard;
                    //sb.Resume();
					
				}
               
                }
                //txtCodigo.Text="Ci: "+ sEnrollNumber.ToString();
                Storyboard sb = this.FindResource("Dias") as Storyboard;
                sb.Begin();
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }

        private void Obtener_Ruta()
        {
            _ruta = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

        }

        private void MostrarFoto(byte[] dbimagen)
        {
            byte[] data = (byte[])dbimagen;

            MemoryStream strm = new MemoryStream();

            strm.Write(data, 0, data.Length);

            strm.Position = 0;

            System.Drawing.Image img = System.Drawing.Image.FromStream(strm);

            BitmapImage bi = new BitmapImage();

            bi.BeginInit();

            MemoryStream ms = new MemoryStream();

            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            ms.Seek(0, SeekOrigin.Begin);

            bi.StreamSource = ms;

            bi.EndInit();

            imgRostro.Source = bi;
        }
        #endregion
        public void PlayVideo(object sender, RoutedEventArgs e)
        {
            VideoPreview.Visibility = Visibility.Collapsed;
            redangVideo.Visibility = Visibility.Visible;
            redangVideo.Play();
        }
        public void PauseVideo(object sender, RoutedEventArgs e)
        {
            VideoPreview.Visibility = Visibility.Collapsed;
            redangVideo.Visibility = Visibility.Visible;
            redangVideo.Pause();
        }
        public void StopVideo(object sender, RoutedEventArgs e)
        {
            VideoPreview.Visibility = Visibility.Collapsed;
            redangVideo.Visibility = Visibility.Visible;
            redangVideo.Stop();
        }
        public  string CalcularFechafin(DateTime dtfechafin) 
        {
            //DateTime dtFechafin = new DateTime(2016, 3, 8, 12, 0, 0);

            DateTime dtHoy = new DateTime();
            
            dtHoy = DateTime.Today;
            
            TimeSpan diffResult = dtfechafin.Subtract(dtHoy);
            string sfechafin2 = diffResult.Days.ToString();
            return (sfechafin2);

            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //Funcion para conectar con un bimetrico
            //Conectar();
            //FUNCION MOSTRAR DATOS DE PRUEBA
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 6);
            dispatcherTimer.Start();
            
          
       }
        int i = 30;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            
                MostrarDatosdePrueba(i);

                i = i + 1;
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
           
         axCZKEM1.Disconnect();
            this.axCZKEM1.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
            this.axCZKEM1.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
            bIsConnected = false;

            Cursor = Cursors.Arrow;           
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
    
        }

        private void redangVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            redangVideo.Position = TimeSpan.Zero;
            redangVideo.LoadedBehavior = MediaState.Play;
            
        }
        
    }
}
