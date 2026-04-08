
using BGestionFAFA;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System.Linq.Expressions;
using System.Text.Json;

namespace FrivGame_Minijuegos_FAFA_APP;

public partial class AdivinarLaPalabra2 : ContentPage
{
    //Ponemos static para que se pueda acceder a esta variable desde cualquier parte de la clase sin necesidad de crear una instancia de la clase
    private string _palabraSecreta = ""; // La palabra secreta modificada (sin acentos y en mayusculas)
    private int _filaActual = 0;
    private int _colActual = 0;
    private string _intentoActual = "";
    private Border[,] _celdas; // un array bidimensional para almacenar las celdas del tablero, cada celda es un Border que contiene un Label con la letra

    #region PROPIEDADES
    public string PalabraSecreta
    {
        get { return _palabraSecreta; }
        set { _palabraSecreta = value; }
    }



    public int FilaActual
    {
        get { return _filaActual; }
        set { _filaActual = value; }
    }

    public int ColActual
    {
        get { return _colActual; }
        set { _colActual = value; }
    }

    public string IntentoPalabraActual
    {
        get { return _intentoActual; }
        set { _intentoActual = value; }
    }

    public Border[,] Celdas
    {
        get { return _celdas; }
        set { _celdas = value; }
    }
    #endregion

    public AdivinarLaPalabra2()
    {
        InitializeComponent();


        ComprobarInternet(); // Comprobamos si hay conexion a internet para cargar la palabra de forma online o offline
        CrearTablero(); // Creamos el tablero adaptandose a la palabra objetivo
        CrearTeclado(); // Creamos el teclado con sus botones y su funcionalidad
    }



    private void RecargarJuego(object sender, EventArgs e)
    {
        ComprobarInternet(); // Comprobamos si hay conexion a internet para cargar la palabra de forma online o offline
        CrearTablero(); // Creamos el tablero adaptandose a la palabra objetivo
        // El teclado no hace falta volver a crearlo porque no cambia
        ButtonReiniciar.IsVisible = false; // Volvemos a ocultar el boton de reiniciar para que no se vea en medio del juego
        LabelMensaje.Text = "";
        FilaActual = 0; // Reseteamos la fila actual para volver a empezar desde la primera fila
        ColActual = 0; // Reseteamos la columna actual para volver a empezar desde la primera columna
        IntentoPalabraActual = ""; // Reseteamos el intento actual para que no se quede guardada la palabra que se estaba escribiendo
        KeyboardLayout.IsEnabled = true; // Volvemos a habilitar el teclado para que se pueda escribir la nueva palabra al haber reiniciado el juego
    }

    private void ComprobarInternet()
    {
        // 1. Comprobamos el estado de la red
        NetworkAccess accesoRed = Connectivity.Current.NetworkAccess;

        //CargarPalabraOffline();

        if (accesoRed == NetworkAccess.Internet)
        {
            // Si hay internet, ejecutamos el método online
            //CargarPalabraOnline();
            PalabraSecreta = ApiWordle.ObtenerPalabraAleatoria(); // Quitamos los acentos de la palabra secreta para que no haya problemas al comparar con el intento del usuario
        }
        else
        {
            // Si no hay internet, ejecutamos el método offline
            //CargarPalabraOffline();
            PalabraSecreta = ApiWordle.CargarPalabraOffline();
        }
    }







    public void CrearTablero()
    {

        // 1. Primero comprobamos el largo de la palabra a adivinar, para ajustar la cantidad de las columnas que necesita la palabra en el tablero 
        int columnas = PalabraSecreta.Length;
        Celdas = new Border[6, columnas]; // el numero de fila no lo tocamos porque siempre va a ver ese numero de intentos de escribir la palabra

        // 2. Limpiamos el Grid por si da problemas al volver a crear el tablero
        GridTablero.Children.Clear();


        // Recorremos las 6 filas(siempre van a ser el numero de oportunidades del programa)
        for (int fila = 0; fila < 6; fila++)
        {
            // Recorremos las columnas
            for (int c = 0; c < columnas; c++)
            {
                // 1. Creamos el texto label que va a contener la letra dentro del cuadro
                Label etiqueta = new Label
                {
                    Text = "",
                    TextColor = Colors.Black,
                    FontSize = 25,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                // 2. Creamos el cuadro con Border y le metemos el texto dentro
                Border cuadro = new Border
                {
                    Stroke = Colors.Gray,
                    StrokeThickness = 2,
                    HeightRequest = 60,
                    WidthRequest = 60,
                    Content = etiqueta // Metemos el texto dentro del cuadro el label
                };



                // 3. Añadimos a nuestra propieda public las celdas añadidas para poder acceder a ellas desde cualquier parte de la clase, por ejemplo para cambiar el texto del label o el color del borde
                Celdas[fila, c] = cuadro;

                // 4. Lo añadimos tambien a nuestro grid para que se muestre en pantalla el tablero
                GridTablero.Add(cuadro, c, fila);
            }
        }
    }

    private void CrearTeclado()
    {

     

        // 1. Definimos las filas con las teclas a usar
        string[][] filasTeclado = new string[][]
        {
            new string[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" },
            new string[] { "A", "S", "D", "F", "G", "H", "J", "K", "L", "Ñ" },
            new string[] { "ENTER", "Z", "X", "C", "V", "B", "N", "M", "<--" }
        };


        // Limpiamos el contenedor antes de generar
        KeyboardLayout.Children.Clear();


        // Vamos a recorrer cada fila del teclado
        foreach (string[] teclasSeparadas in filasTeclado)
        {
            // 2. Creamos un Grid para la fila 
            Grid fila = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                ColumnSpacing = 2,
                Margin = new Thickness(0, 2)
            };

            // 3. Recorremos todas las teclas de dicha fila de array 
            for (int i = 0; i < teclasSeparadas.Length; i++)
            {
             
                // Alargamos tanta veces columnas como letras haya en esa fila
                fila.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
            }

            // 4. Creamos un botón para cada tecla y lo agregamos a la fila
            for (int i = 0; i < teclasSeparadas.Length; i++)
            {
                string texto = teclasSeparadas[i];

                // Creamos el botón con tus propiedades originales
                Button botonNuevo = new Button
                {
                    Text = texto,
                    BackgroundColor = Colors.LightGray,
                    TextColor = Colors.Black,
                    CornerRadius = 5,
                    Padding = 0, // Añadido para que el texto largo no se corte
                    FontSize = 14,

        
                };
 
                // Añadimos a cada boton un evento centralizado para cuando se pulse cualquiera tecla se gestione desde el mismo metodo
                botonNuevo.Clicked += (s, e) => PresionarTecla(botonNuevo);

                // Añadimos el boton al Grid en la columna correspondiente
                fila.Add(botonNuevo, i, 0);
            }

            // Añadimos la fila completa al diseño del teclado (KeyboardLayout)
            KeyboardLayout.Add(fila);
        }

    }

    public void PresionarTecla(Button boton)
    {
        string teclaPulsada = boton.Text;

        // Usamos switch para que sea más visible y fácil de organizar
        switch (teclaPulsada)
        {
            case "ENTER":
                // Comprobamos que haya puesto una palabra 
                if (IntentoPalabraActual.Length == PalabraSecreta.Length )
                {
                    ValidarPalabra();
                }
                break;

            case "<--":
                // Borramos la ultima letra escrita
                BorrarUltimaLetra();
                break;

            default:
                // Si no es Enter ni Borrar, asumimos que es una letra
                EscribirLetra(teclaPulsada);
                break;
        }

    }

    private async void EscribirLetra(string teclaPulsada)
    {
        // Comprobamos si no hemos superado el largo de la palabra secreta
        if (IntentoPalabraActual.Length < PalabraSecreta.Length)
        {
            // 1. Localizamos la celda visual usando nuestros índices
            Border borderActual = Celdas[FilaActual, ColActual];  // La primera vez seria la 0, 0

            // 2. Sacamo el Label que está dentro del Border para poder cambiar su texto
            Label labelActual = (Label)borderActual.Content;

            // 3. Editamo el contenido del Label para mostrar la letra que se ha pulsado
            labelActual.Text = teclaPulsada;

            // Cambiamos el color del borde para indicar que la celda ya tiene una letra
            borderActual.Stroke = Colors.Black;

            // 4. A la palabra actual le añadimos la letra que se ha pulsado para ir formando la palabra que se va a ir escribiendo
            IntentoPalabraActual = IntentoPalabraActual + teclaPulsada;
            ColActual = ColActual + 1; // Cada vez que escribimos una leta le sumamo uno a las propieda de columna actual para la proxima letra que se escriba

            // 5. Animación simple simulando un rebote al escribir la leta
            // Lo ponemos await para que no se ejecute la 2 animacions a la vez y se ejecute en orden para que no se solapen
            await borderActual.ScaleTo(1.1, 50); // Crece un 10%
            await borderActual.ScaleTo(1.0, 50); // Vuelve a su tamaño

       
        }
    }

    private void BorrarUltimaLetra()
    {
        // Si no hay niguna letra escrita que no borremos nada
        if (IntentoPalabraActual.Length > 0)
        {
            // Retrocedemos una columna para posicionarno en la celda ultima escrita
            ColActual -= 1;

            Celdas[FilaActual, ColActual].Stroke = Colors.Gray; // Le devolvemos el color que tenia que tener el borde del Border 

            // Sacamos el label de la celda para cambiar su texto a vacio
            Label labelActual = (Label)Celdas[FilaActual, ColActual].Content; // Sacamos el label de la celda para cambiar su texto

            labelActual.Text = ""; // Borramos el contenido

            // Actualizar la IntentoPalabraActual quitando su ultima letra puesta
            IntentoPalabraActual = IntentoPalabraActual.Remove(IntentoPalabraActual.Length - 1);
            // El Remove quita una parte del string, le pasamos la posición desde donde queremos quitar, en este caso quitamos 1 caracter desde la ultima posición

        }

    }

    // TODO: MEJORAR EN LA VALIDACION LA REPETICION DE LETRAS, PORQUE SI HAY LETRAS REPETIDAS EN LA PALABRA SECRETA NO SE ESTA VALIDANDO BIEN,
    // PORQUE SI LA LETRA EXISTE EN LA PALABRA PERO YA SE HA VALIDADO ANTERIORMENTE COMO VERDE O AMARILLA, LAS SIGUIENTES VECES QUE APAREZCA ESA LETRA NO SE DEBERIA VALIDAR O DEBERIA VALIDAR COMO GRIS,
    // PERO AHORA MISMO SI HAY UNA LETRA REPETIDA EN LA PALABRA SECRETA Y EL USUARIO ESCRIBE ESA LETRA EN SU INTENTO, SIEMPRE SE VA A VALIDAR COMO AMARILLA O VERDE DEPENDIENDO DE SI ESTA EN LA POSICION CORRECTA O NO,
    // PERO NO SE ESTA TENIENDO EN CUENTA SI ESA LETRA YA SE HA VALIDADO ANTERIORMENTE Y YA NO QUEDAN MAS INSTANCIAS DE ESA LETRA POR VALIDAR
    private async void ValidarPalabra()
    {
        try
        {
            // Comprobamos si la palabra escrita existe en el diccionario, si no existe se lanzará una excepción que se capturará en el catch para mostrar un mensaje de error al usuario
            if (!ApiWordle.ComprobarSiExiste(IntentoPalabraActual))
            {
                LabelMensaje.TextColor = Colors.Green;
                throw new Exception("La palabra no existe en el diccionario, prueba con otra palabra");
            }


            // 1. Recorremos cada letra usando directamente el largo de la palabra secreta
            for (int i = 0; i < PalabraSecreta.Length; i++)
            {
                // Nos situamos en la fila que estamos ahora y vamos a recorrer sus columnas gracias a indice 
                Border border = Celdas[FilaActual, i];

                // Sacamos el label de la celda para comprobar la letra que hay escrita y para cambiar su color dependiendo del resultado de la comparación
                Label label = (Label)border.Content;


                // Comparamos la letra del intento con la de la palabra secreta
                char letraIntento = IntentoPalabraActual[i];

                // Animacion que girda las palabras una detras de otra 
                await border.RotateYTo(90, 150);

                // Comprobamos la letra actual escrita, con la de la misma posicion que hay en la palabra
                // Si es igual la ponemos de color verde
                if (letraIntento == PalabraSecreta[i])
                {
                    // VERDE: Posición exacta
                    border.BackgroundColor = Colors.Green;
                    border.Stroke = Colors.Green;
                }
                else if (PalabraSecreta.Contains(letraIntento)) // Sino es igual, pero la letra existe en la palabra secreta, la ponemos de color amarillo
                {
                    // AMARILLO: Existe pero en otro sitio
                    border.BackgroundColor = Colors.Gold;
                    border.Stroke = Colors.Gold;
                }
                else
                {
                    // GRIS: No existe
                    border.BackgroundColor = Colors.Gray;
                    border.Stroke = Colors.Gray;
                }

                // Cambiamos el color de todos
                label.TextColor = Colors.White;

                // Animación de giro
                await border.RotateYTo(0, 150);
            }
 
            // 2. Comprobar resultado final usando las propiedades y actualizando el Label de la interfaz
            if (IntentoPalabraActual == PalabraSecreta)
            {
                LabelMensaje.TextColor = Colors.Green;
                KeyboardLayout.IsEnabled = false; // Deshabilitamos el teclado para que no pueda seguir escribiendo al haber ganado
                ButtonReiniciar.IsVisible = true; // Hacemos visible el boton de reiniciar para que pueda volver a jugar
                throw new Exception("¡ENHORABUENA! HAS ACERTADO!");
            }
            else
            {
                // Avanzamos fila y reseteamos columnas/intento
                FilaActual = FilaActual + 1;
                ColActual = 0;
                IntentoPalabraActual = "";

                if (FilaActual == 6)
                {
                    LabelMensaje.TextColor = Colors.Red;
                    KeyboardLayout.IsEnabled = false; // Deshabilitamos el teclado para que no pueda seguir escribiendo al haber ganado
                    ButtonReiniciar.IsVisible = true; // Hacemos visible el boton de reiniciar para que pueda volver a jugar
                    throw new Exception("FIN DEL JUEGO. LA PALABRA ERA: " + PalabraSecreta);


                }
                else
                {
                    LabelMensaje.TextColor = Colors.Gray;
                    LabelMensaje.Text="¡INTENTA DE NUEVO! TE QUEDAN " + (6 - FilaActual) + " INTENTOS";
                }
           
            }
        }
        catch (Exception error) { 
        
            LabelMensaje.Text = error.Message;
            

        }
    }


}