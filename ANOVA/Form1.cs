using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ANOVA
{
    public partial class Form1 : Form
    {
        int BrojMjerenja = 0;
        int BrojAlternativa = 0;
        private int BrojKolona;
        private int BrojVrsta;
        
        List<string> rezultati = new List<string>();
        double[,] errors;

        Chart formulaZaTabelarno = new Chart();

        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void button1_Click(object sender, EventArgs e)
        { 
            int.TryParse(MjerenjaBox.Text, out BrojMjerenja);
            int.TryParse(AlternativaBox.Text, out BrojAlternativa);
            //MessageBox.Show($"Kolona:{BrojKolona}\n Vrsta:{BrojVrsta}");
            if (BrojMjerenja < 1 || BrojAlternativa<1)
            {
                MessageBox.Show($"Nedovoljan broj vrsta ili kolona unesen!");
            }
            else
            {
                BrojVrsta =BrojMjerenja + 2;
                BrojKolona =BrojAlternativa + 2;
                //MessageBox.Show($"broj vrsta!{BrojVrsta} ");
                dataGridView1.ColumnCount = BrojKolona;
                dataGridView1.RowCount = BrojVrsta;
                dataGridView1.Columns[0].Name = "Mjerenja";
                for (int i=1;i<BrojKolona;i++)
                {
                    dataGridView1.Columns[i].Name = "Alternativa " + i.ToString();
                }
                dataGridView1.Columns[BrojKolona-1].Name = "Ukupna srednja vrijednost";
                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.CurrentCell = dataGridView1[0, BrojVrsta-2];
                dataGridView1.CurrentCell.Value = "Srednja vrijednost";
                dataGridView1.CurrentCell = dataGridView1[0, BrojVrsta - 1];
                dataGridView1.CurrentCell.Value = "Efekat";
                dataGridView1.Rows[BrojVrsta - 2].ReadOnly = true;
                dataGridView1.Rows[BrojVrsta - 1].ReadOnly = true;
                dataGridView1.Columns[BrojKolona-1].ReadOnly = true;
                for (int i=0;i<BrojVrsta-2;i++)
                {
                    dataGridView1.CurrentCell = dataGridView1[0, i];
                    dataGridView1.CurrentCell.Value = (i + 1).ToString();
                }
                for(int i=1;i<BrojKolona;i++)
                {
                    for (int j = 0; j < BrojVrsta; j++)
                    {
                        dataGridView1.CurrentCell = dataGridView1[i, j];
                        dataGridView1.CurrentCell.Value = 0;
                    }
                }
                for (int i = 0; i < BrojVrsta ; i++)
                {
                    if (i == BrojVrsta - 2)
                        continue;
                    dataGridView1.CurrentCell = dataGridView1[BrojKolona - 1, i];
                    dataGridView1.CurrentCell.Value = " ";
                }

                dataGridView1.CurrentCell = dataGridView1[0, 0];//errors = new double[BrojMjerenja,BrojAlternativa];
                

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double prosjek = 0;
            double UkupanProsjektmp = 0;
            double UkupanProsjek = 0;
            double suma = 0;
            double efekat = 0;
            double TrenutnaVrijednost=0;
             for (int i = 1; i < BrojKolona - 1; i++)

                {
                for (int j = 0; j < BrojVrsta - 2; j++)
                {
                    this.dataGridView1.CurrentCell = this.dataGridView1[i, j];
                    double.TryParse(dataGridView1.CurrentCell.Value.ToString(), out TrenutnaVrijednost);
                   suma += TrenutnaVrijednost;
                    TrenutnaVrijednost = 0;
                }
                prosjek = suma / (double)BrojMjerenja;
                suma = 0;
                dataGridView1.CurrentCell = this.dataGridView1[i, BrojVrsta - 2];

                dataGridView1.CurrentCell.Value = (double)prosjek;
                UkupanProsjektmp += prosjek;
            }
            UkupanProsjek = UkupanProsjektmp /(double) BrojAlternativa;
            dataGridView1.CurrentCell = this.dataGridView1[BrojKolona-1, BrojVrsta - 2];
            dataGridView1.CurrentCell.Value = (double)UkupanProsjek;
           
            for(int i=1;i<BrojKolona-1;i++)
            {
                double tmp = 0;
                this.dataGridView1.CurrentCell = this.dataGridView1[i, BrojVrsta - 2];
                double.TryParse(dataGridView1.CurrentCell.Value.ToString(),out tmp);
                efekat = tmp - UkupanProsjek;
                dataGridView1.CurrentCell = dataGridView1[i, BrojVrsta - 1];
                dataGridView1.CurrentCell.Value = efekat;
               
            }

            double SSA = 0;
            double SSE = 0;
            double SST = 0;
            int stSlobodeAlternativa = 0;//ssa k-1,k broj alternativa
            int stSlobodeGreske = 0;//sse  k(n-1)
            int stSlobodeTotala = 0;//sst kn-1
            double VarijansaAlternativa = 0;
            double VarijansaGreske = 0;
            double IzracunatoF = 0;
            double TabelarnoF = 0;
            List<double> kontrasti = izracunajKontraste();
            double donjiInterval = 0;
            double gornjiInterval = 0;
            double sc = 0;
            SSA = IzracunajSSA();
            rezultati.Add("SSA -> " + (Math.Round(SSA,5)).ToString());
            SSE = IzracunajSSE();
            rezultati.Add("SSE -> " + (Math.Round(SSE, 5)).ToString());
            SST = SSE + SSA;
            rezultati.Add("SST -> " + (Math.Round(SST, 5)).ToString());
            stSlobodeAlternativa = BrojAlternativa - 1;
            rezultati.Add("Stepeni slobode alternativa -> " + stSlobodeAlternativa.ToString());
            stSlobodeGreske = BrojAlternativa * (BrojMjerenja - 1);
            rezultati.Add("Stepeni slobode Greske -> " + stSlobodeGreske.ToString());
            stSlobodeTotala = BrojAlternativa * BrojMjerenja - 1;
            rezultati.Add("Stepeni slobode Totala -> " + stSlobodeTotala.ToString());
            VarijansaAlternativa = SSA / (double)stSlobodeAlternativa;
            rezultati.Add("Varijansa alternativa -> " + (Math.Round(VarijansaAlternativa, 5)).ToString());
            VarijansaGreske = SSE / (double)stSlobodeGreske;
            rezultati.Add("Varijansa greske -> " + (Math.Round(VarijansaGreske, 5)).ToString());
            IzracunatoF = VarijansaAlternativa / VarijansaGreske;
            rezultati.Add("F izracunato -> " + (Math.Round(IzracunatoF, 5)).ToString());
            TabelarnoF = formulaZaTabelarno.DataManipulator.Statistics.InverseFDistribution(0.05,stSlobodeAlternativa,stSlobodeGreske);//95%=1-0,95
            rezultati.Add("F tabelarno -> " + (Math.Round(TabelarnoF, 5)).ToString());
            if(IzracunatoF>TabelarnoF)
            {
                rezultati.Add("Razlike izmedju alternativa su statisticki znacajne");
            }
            else 
            {
                rezultati.Add("Razlike izmedju alternativa nisu statisticki znacajne");
            }
            kontrasti = izracunajKontraste();
            sc = Math.Sqrt((2 * VarijansaGreske) / (double)(BrojAlternativa * BrojMjerenja));
            double tValue = formulaZaTabelarno.DataManipulator.Statistics.InverseTDistribution(0.05,stSlobodeGreske);
            int lijevo = 1;
            int desno = 2;
            for (int i = 0; i < kontrasti.Count; i++)
            {
                donjiInterval = kontrasti[i] - tValue * sc;
                gornjiInterval = kontrasti[i] + tValue * sc;
                donjiInterval=Math.Round(donjiInterval, 5);
                gornjiInterval=Math.Round(gornjiInterval, 5);
                rezultati.Add($"Kontrasti ({lijevo},{desno}) = ({donjiInterval},{gornjiInterval})");
                desno = (desno + 1) % (BrojAlternativa + 1);
                if (lijevo == 0)
                {
                    lijevo++;
                    lijevo = lijevo + 1;
                }
            }

            Ispis(rezultati);

           
          

        }
        private void Ispis(List<string> rezultati)
        {
            foreach(string linija in rezultati)
            {
                RezultatiBox.AppendText(linija);
                RezultatiBox.AppendText("\n");
            }
        }
        private List<double> izracunajKontraste()
        {
            List<double> Lista = new List<double>();
            double alfa1=0;
            double alfa2 = 0;
            for (int i=0;i<BrojAlternativa-1;i++)
            {
                for(int j=i+1;j<BrojAlternativa;j++)
                {
                    dataGridView1.CurrentCell = dataGridView1[i + 1, BrojVrsta-1];
                    double.TryParse( dataGridView1.CurrentCell.Value.ToString(),out alfa1);
                    dataGridView1.CurrentCell = dataGridView1[j+1, BrojVrsta - 1];
                    double.TryParse(dataGridView1.CurrentCell.Value.ToString(), out alfa2);
                    Lista.Add(alfa1 - alfa2);

                }
            }
            return Lista;
            
        }
        private double IzracunajSSA()
        {
            double sumaEfekata = 0;
            double SSA = 0;
            for (int i = 1; i < BrojKolona - 1; i++)
            {
                double efekti = 0;
                this.dataGridView1.CurrentCell = this.dataGridView1[i, BrojVrsta - 1];
                double.TryParse(dataGridView1.CurrentCell.Value.ToString(), out efekti);
                sumaEfekata += (efekti*efekti);
            }
            SSA = BrojMjerenja * sumaEfekata;
            return SSA;

        }
        private double IzracunajSSE()
        {
            double SSE = 0;
            for (int i = 0; i < BrojAlternativa; i++)
            {
                for (int j = 0; j < BrojMjerenja; j++)
                {
                    dataGridView1.CurrentCell = dataGridView1[i + 1, j];
                    double VrijednostCelije = 0;
                    double SrednjaVrijednostKolone = 0;
                    double.TryParse(dataGridView1.CurrentCell.Value.ToString(), out VrijednostCelije);
                    dataGridView1.CurrentCell = dataGridView1[i + 1, BrojVrsta - 2];
                    double.TryParse(dataGridView1.CurrentCell.Value.ToString(), out SrednjaVrijednostKolone);
                    double tmp = (VrijednostCelije - SrednjaVrijednostKolone)* (VrijednostCelije - SrednjaVrijednostKolone);
                    SSE += tmp;
                }
            }
            return SSE;
        }

        private void RezultatiBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
