using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Calculos
{
    public partial class Form1 : FormValidated
    {
        private double altoCubierta;
        private double diametro;
        private double circunsferencia;
        private double vueltasRuedaCorona;
        private double vueltasCajaPiñon;
        private double relacionCajaMotor;
        private double relacionPiñonCorona;

        private double newRPM;
        private double newKMH;
        private double newPiñon;
        private double newCorona;
        private double newRuedaTamaño;
        private double newAncho;
        private double newAlto;

        public Form1()
        {
            InitializeComponent();
            errorProvider = new ErrorProvider();

            altoCubierta = 0;
            diametro = 0;
            circunsferencia = 0;
            vueltasRuedaCorona = 0;
            vueltasCajaPiñon = 0;
            relacionCajaMotor = 0;
            relacionPiñonCorona = 0;

            newRPM = 0;
            newKMH = 0;
            newPiñon = 0;
            newCorona = 0;
            newRuedaTamaño = 0;
            newAncho = 0;
            newAlto = 0;
            
            txtAlto.Text = 70.ToString("#0.00");
            txtAncho.Text = 130.ToString("#0.00");
            txtCorona.Text = 37.ToString("#0.00");
            txtPiñon.Text = 13.ToString("#0.00");
            txtRPM.Text = 10800.ToString("#0.00");
            txtRuedaTamaño.Text = 17.ToString("#0.00");
            txtVelocidad.Text = 145.ToString("#0.00");

            txtNewAlto.Text = 70.ToString("#0.00");
            txtNewAncho.Text = 150.ToString("#0.00");
            txtNewCorona.Text = 34.ToString("#0.00");
            txtNewPiñon.Text = 13.ToString("#0.00");
            txtNewRuedaTamaño.Text = 17.ToString("#0.00");
            txtNewVelocidad.Text = 150.ToString("#0.00");

            CalculateOriginal();
        }

        private void CalculateOriginal()
        {
            if (!IsValid(Controls))
                return;

            if (!string.IsNullOrEmpty(txtAncho.Text) && !string.IsNullOrEmpty(txtAlto.Text))
            {
                altoCubierta = (Convert.ToDouble(txtAncho.Text)/10)*(Convert.ToDouble(txtAlto.Text)/100);

                if (!string.IsNullOrEmpty(txtRuedaTamaño.Text))
                {
                    diametro = (Convert.ToDouble(txtRuedaTamaño.Text) * 2.54) + (altoCubierta*2);
                    circunsferencia = diametro*Math.PI;
                    txtRuedaCircunsferencia.Text = circunsferencia.ToString("#0.00");
                }
                if (!string.IsNullOrEmpty(txtVelocidad.Text))
                    vueltasRuedaCorona = Convert.ToDouble(txtVelocidad.Text)*100000/circunsferencia;
                if (!string.IsNullOrEmpty(txtCorona.Text) && !string.IsNullOrEmpty(txtPiñon.Text))
                    relacionPiñonCorona = Convert.ToDouble(txtCorona.Text)/Convert.ToDouble(txtPiñon.Text);
                if (vueltasRuedaCorona > 0 && relacionPiñonCorona > 0)
                    vueltasCajaPiñon = vueltasRuedaCorona*relacionPiñonCorona;
                if (vueltasCajaPiñon > 0 && !string.IsNullOrEmpty(txtRPM.Text))
                    relacionCajaMotor = Convert.ToDouble(txtRPM.Text)*60/vueltasCajaPiñon;
            }
            txtVueltasRuedaCorona.Text = vueltasRuedaCorona.ToString("#,##0.00");
            txtVueltasCajaPiñon.Text = vueltasCajaPiñon.ToString("#,##0.00");
            txtRelacionesCajaMotor.Text = relacionCajaMotor.ToString("#,##0.00");
            txtRelacionesPiñonCorona.Text = relacionPiñonCorona.ToString("#,##0.00");
            CalculateNew();
        }

        private void CalculateNew()
        {
            if (!IsValid(Controls))
                return;

            if(!string.IsNullOrEmpty(txtNewAlto.Text) && Convert.ToDouble(txtNewAlto.Text) > 0
                && !string.IsNullOrEmpty(txtNewAncho.Text) && Convert.ToDouble(txtNewAncho.Text) > 0
                && !string.IsNullOrEmpty(txtNewCorona.Text) && Convert.ToDouble(txtNewCorona.Text) > 0
                && !string.IsNullOrEmpty(txtNewPiñon.Text) && Convert.ToDouble(txtNewPiñon.Text) > 0
                && !string.IsNullOrEmpty(txtNewRuedaTamaño.Text) && Convert.ToDouble(txtNewRuedaTamaño.Text) > 0
                && !string.IsNullOrEmpty(txtNewVelocidad.Text) && Convert.ToDouble(txtNewVelocidad.Text) > 0)
            {
                double newCircunsferencia = ((Convert.ToDouble(txtNewRuedaTamaño.Text) * 2.54) + ((Convert.ToDouble(txtNewAncho.Text) / 10) * (Convert.ToDouble(txtNewAlto.Text) / 100) * 2)) * Math.PI;
                double newVueltasRuedaCorona = Convert.ToDouble(txtNewVelocidad.Text) * 100000 / newCircunsferencia;
                double newRelacionPiñonCorona = Convert.ToDouble(txtNewCorona.Text) / Convert.ToDouble(txtNewPiñon.Text);
                double newVueltasCajaPiñon = newVueltasRuedaCorona * newRelacionPiñonCorona;
                double newRPM = newVueltasCajaPiñon*relacionCajaMotor/60;

                txtNewCircunsferencia.Text = newCircunsferencia.ToString("#0.00");
                txtNewRPM.Text = newRPM.ToString("#,##0");

                GenerateGraph(newCircunsferencia, newRelacionPiñonCorona);
            }
        }

        private void GenerateGraph(double newCircunsferencia, double newRelacionPiñonCorona)
        {
            chGraph.Series.Clear();
            List<SpeedByRPM> result = new List<SpeedByRPM>();
            for(int rpm = 5000; rpm <= 11000; rpm+=100)
            {
                double velocidad = rpm * 60 / relacionCajaMotor / newRelacionPiñonCorona * newCircunsferencia / 100000;
                result.Add(new SpeedByRPM(velocidad, rpm));
            }
            chGraph.Series.Add(GetStatisticalData(result));
        }

        public Series GetStatisticalData(List<SpeedByRPM> lst)
        {
            Series statisticData = new Series("Velocidad por RPM");
            statisticData.IsXValueIndexed = true;
            statisticData.ChartType = SeriesChartType.Line;
            chGraph.GetToolTipText += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs>(chGraph_GetToolTipText);

            foreach (SpeedByRPM speedByRpm in lst)
                statisticData.Points.Add(new DataPoint(speedByRpm.RPM, speedByRpm.Speed));
            return statisticData;
        }

        private void txtRPM_TextChanged(object sender, EventArgs e)
        {
            CalculateOriginal();
        }

        private void txtVelocidad_TextChanged(object sender, EventArgs e)
        {
            CalculateOriginal();
        }

        private void txtPiñon_TextChanged(object sender, EventArgs e)
        {
            CalculateOriginal();
        }

        private void txtCorona_TextChanged(object sender, EventArgs e)
        {
            CalculateOriginal();
        }

        private void txtRuedaTamaño_TextChanged(object sender, EventArgs e)
        {
            CalculateOriginal();
        }

        private void txtAncho_TextChanged(object sender, EventArgs e)
        {
            CalculateOriginal();
        }

        private void txtAlto_TextChanged(object sender, EventArgs e)
        {
            CalculateOriginal();
        }

        private void txtRuedaCircunsferencia_TextChanged(object sender, EventArgs e)
        {
            CalculateOriginal();
        }

        private void txtNewVelocidad_TextChanged(object sender, EventArgs e)
        {
            CalculateNew();
        }

        private void txtNewPiñon_TextChanged(object sender, EventArgs e)
        {
            CalculateNew();
        }

        private void txtNewCorona_TextChanged(object sender, EventArgs e)
        {
            CalculateNew();
        }

        private void txtNewRuedaTamaño_TextChanged(object sender, EventArgs e)
        {
            CalculateNew();
        }

        private void txtNewAncho_TextChanged(object sender, EventArgs e)
        {
            CalculateNew();
        }

        private void txtNewAlto_TextChanged(object sender, EventArgs e)
        {
            CalculateNew();
        }

        private void txtRPM_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtRPM, "Ingrese solo numeros");
        }

        private void txtVelocidad_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtVelocidad, "Ingrese solo numeros");
        }

        private void txtPiñon_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtPiñon, "Ingrese solo numeros");
        }

        private void txtCorona_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtCorona, "Ingrese solo numeros");
        }

        private void txtRuedaTamaño_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtRuedaTamaño, "Ingrese solo numeros");
        }

        private void txtAncho_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtAncho, "Ingrese solo numeros");
        }

        private void txtAlto_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtAlto, "Ingrese solo numeros");
        }

        private void txtNewVelocidad_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtNewVelocidad, "Ingrese solo numeros");
        }

        private void txtNewPiñon_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtNewPiñon, "Ingrese solo numeros");
        }

        private void txtNewCorona_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtNewCorona, "Ingrese solo numeros");
        }

        private void txtNewRuedaTamaño_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtNewRuedaTamaño, "Ingrese solo numeros");
        }

        private void txtNewAncho_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtNewAncho, "Ingrese solo numeros");
        }

        private void txtNewAlto_Validating(object sender, CancelEventArgs e)
        {
            Validation_Price(txtNewAlto, "Ingrese solo numeros");
        }

        private void chGraph_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            // Check selevted chart element and set tooltip text
            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                int i = e.HitTestResult.PointIndex;
                DataPoint dp = e.HitTestResult.Series.Points[i];
                e.Text = string.Format("{0:F1}, {1:F1}", dp.XValue, dp.YValues[0]);
            }
        }
    }
}
