using System;
using System.Collections.Generic;
using System.Text;

namespace RNAHopfield
{
    public class RedHopfield
    {
        Neurona[] neuronas;

        double energiaInicial;
        public double EnergiaInicial
        {
            get { return energiaInicial; }
            set { energiaInicial = value; }
        }

        double energiaFinal;
        public double EnergiaFinal
        {
            get { return energiaFinal; }
            set { energiaFinal = value; }
        }

        // double[] es un vector de datos de entrada (de informaci�n) con el que se entrenar�
        // e igual al n�mero de vectores de informaci�n M
        // (double[])[] es un vector de entradas igual a la cantidad de neuronas N
        public void entrenar(double[][] vectoresInformacion)
        {
            int N = vectoresInformacion[0].Length;
            int M = vectoresInformacion.Length;

            int i, j, k;

            //Carga la matriz de pesos para que sea sim�trica y contenga los productos buscados
            for (i = 0; i < N; i++)
            {
                //De esta forma nos aseguramos que no entrar� en la diagonal
                for (j = i + 1; j < N; j++)
                {
                    for (k = 0; k < M; k++)
                    {
                        neuronas[i].Peso[j-1] += vectoresInformacion[k][i] * vectoresInformacion[k][j];
                        neuronas[j].Peso[i] += vectoresInformacion[k][i] * vectoresInformacion[k][j];

                        // Con respecto a la exportaci�n al archivo de pesos,
                        // se podr�a incorporar una salida para el caso de entrenar
                        // o, en un sentido purista, memorizar
                        // exportador.write(0,1,0,i,j-1);
                        // exportador.write(0,1,0,j,i);
                    }
                }
            }            
        }


        public RedHopfield(double[][] vectoresInformacion)
        {
            int N = vectoresInformacion[0].Length;

            int i, j, k;

            // Crea una red de tama�o n, basado en el tama�o de la informaci�n
            // a memorizar            
            neuronas = new Neurona[N];

            // Crea las neuronas de la red con sus enlaces y pesos
            for (i = 0; i < N; i++)
            {
                neuronas[i] = new Neurona("Neurona " + (i + 1));
                neuronas[i].Peso = new double[N - 1];
                neuronas[i].Enlace = new Neurona[N - 1];
            }

            for (i = 0; i < N; i++)
            {
                for (j = 0; j < i; j++)
                {
                    neuronas[i].Enlace[j] = neuronas[j];
                }
                for (j = i + 1; j < N; j++)
                {
                    neuronas[i].Enlace[j - 1] = neuronas[j];
                }                
            }

            // Entrena la red neuronal
            entrenar(vectoresInformacion);            
        }

        // Recibe un vector de entradas y produce salida cuando la variaci�n de los pesos cumpla
        // la norma c�bica (cuando todos sean menores a un �psilon)
        public double[] calcular(double[] vectorEntrada)
        {
            int i,j;
            bool cumpleNorma;

            // Por lo pronto, que no calcule energia.
            //energiaInicial = calcularEnergia(vectorEntrada);

            for (i = 0; i < neuronas.Length; i++)
            {
                neuronas[i].ValorActual = vectorEntrada[i];
            }

            #region Inicializacion del Exportador
            // El siguiente pedazo de c�digo inicializa el archivo
            ExportadorHelper exportador = new ExportadorHelper("CALCULAR");
            #endregion

            j = 0;
            do
            {
                System.Console.WriteLine("Vuelta " + j + ":");
                j++;
                
                cumpleNorma = true;

                for (i = 0; i < neuronas.Length; i++)
                {
                    //System.Console.WriteLine("" + neuronas[i].ValorActual);

                    neuronas[i].procesar();
                    cumpleNorma = cumpleNorma && neuronas[i].cumpleNorma();

                    // La siguiente instrucci�n actualiza el valor presente de la
                    // neurona, de modo que los valores que se calculen para las
                    // neuronas restantes tendr�n valores distintos seg�n el �rden
                    // de c�lculo. En este caso, no se eligi� esa metodolog�a

                    neuronas[i].actualizar();

                    // Para exportar, queda ver con Juan Carlos Vazquez qu� poner
                    // en cada par�metro. Por lo pronto, lo exporta para cada vuelta
                    // y no para cada neurona, que deberia ir a continuacion    
                                    
                    // Aqui Impresion de la red entera                    
                    //#region Impresion de la red entera
                    //for (int h = 0; h < neuronas.Length; h++)
                    //{
                    //                     etapa  juego  capa  neurona  peso                              valor actual 
                    //    exportador.write(    j,     i,    0,       h,    0, Convert.ToInt16(neuronas[h].ValorActual));
                    //}
                    //#endregion
                }
                
                // El siguiente ciclo permite actualizar la red entera, de forma
                // tal que primero se calculan los valores futuros para todas las
                // neuronas para sus valores actuales y luego cambiar todos de una
                // sola vez
                //for (i = 0; i < neuronas.Length; i++)
                //{
                //    neuronas[i].actualizar();
                //}

                // Para exportar, queda ver con Juan Carlos Vazquez qu� poner
                // en cada par�metro. Por lo pronto, lo exporta para cada vuelta,
                // que deberia ir a continuacion   
                #region Impresion de la red entera
                for (int h = 0; h < neuronas.Length; h++)
                {
                                   //etapa  juego  capa  neurona  peso                              valor actual 
                    exportador.write(    j,     i,    0,       h,    0, Convert.ToInt16(neuronas[h].ValorActual));
                }
                #endregion

                                     
                //System.Console.WriteLine();
            }
            while (!cumpleNorma);

            //System.Console.WriteLine("Salida:");
            //for (i = 0; i < neuronas.Length; i++)
            //{
            //    System.Console.WriteLine(neuronas[i].ValorActual);
            //}

            double[] res = new double[neuronas.Length];

            for (i = 0; i < neuronas.Length; i++)
            {
                res[i] = neuronas[i].ValorActual;
            }

            // Por lo pronto, que no calcule energia.
            //energiaFinal = calcularEnergia(res);

            #region Destrucci�n del Exportador
            // El siguiente pedazo de c�digo cierra el archivo
            exportador.close();
            #endregion

            return res;
        }

        protected double calcularEnergia(double[] vectorActual)
        {
            double resultado =0;
            int i, j;

            for (i = 0; i < neuronas.Length; i++)
            {
                for (j = 0; j < i; j++)
                {
                    resultado += neuronas[i].Peso[j]*vectorActual[i]*vectorActual[j];
                }
                for (j = i + 1; j < neuronas.Length; j++)
                {
                    resultado += neuronas[i].Peso[j - 1] * vectorActual[i] * vectorActual[j];                    
                }
            }

            return ((-0.5f)*resultado);
        }
    }
}
