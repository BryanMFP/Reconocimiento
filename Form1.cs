using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace Reconocimiento
{
    public partial class Form1 : Form
    {
        int con = 0;
        Image<Bgr, byte> currentFrame;
        Capture Grabar;
        HaarCascade face;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX,0.4d, 0.4d);
        Image<Gray, byte> result = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> NombrePersonas = new List<string>();
        int ContTrain, Numlabels, t;
        string Nombre;
        DataGridView d = new DataGridView();

        public Form1()
        {
            InitializeComponent();
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
                clsDA.Consultar(d);
                string[] Labels = clsDA.Nombre;
                Numlabels = clsDA.TotalRostros;
                ContTrain = Numlabels;
                for (int i = 0; i < Numlabels; i++)
                {
                    con = i;
                    Bitmap bmp = new Bitmap(clsDA.ConvertBinaryToImg(con));
                    trainingImages.Add(new Image<Gray, byte>(bmp));
                    labels.Add(Labels[i]);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Sin Rostros para cargar");
            }
        }

        private void FrameGrabar(object sender, EventArgs e)
        {
            lblcantidad.Text = "0";
            NombrePersonas.Add("");

            try {
                currentFrame = Grabar.QueryFrame().Resize(320, 240, INTER.CV_INTER_CUBIC);
                gray = currentFrame.Convert<Gray, Byte>();
                MCvAvgComp[][] RostrosDetectados = gray.DetectHaarCascade(face, 1.5,10,HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20,20));

                foreach (MCvAvgComp R in RostrosDetectados[0])
                {
                    t = t + 1;
                    result = currentFrame.Copy(R.rect).Convert<Gray, Byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    currentFrame.Draw(R.rect, new Bgr(Color.Green), 1);

                    if (trainingImages.ToArray().Length != 0) {
                        MCvTermCriteria criterio = new MCvTermCriteria(ContTrain, 0.88);
                        EigenObjectRecognizer recogida = new EigenObjectRecognizer(trainingImages.ToArray(), labels.ToArray(),ref criterio);
                        var fa = new Image<Gray, Byte>[trainingImages.Count];

                        Nombre = recogida.Recognize(result);

                        currentFrame.Draw(Nombre, ref font, new Point(R.rect.X -2,));
                    }
                }
            }
            catch (){ }

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
