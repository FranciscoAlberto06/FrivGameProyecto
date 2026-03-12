
using System.Text.Json;

namespace FrivGame_Minijuegos_FAFA_APP;

public partial class AdivinarLaPalabra2 : ContentPage
{
    //Ponemos static para que se pueda acceder a esta variable desde cualquier parte de la clase sin necesidad de crear una instancia de la clase
    private string _palabraSecreta = "";
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
        CargarPalabra(); // Generamos una palabra aleatoria que encontre en la api de la RAE
        CrearTablero(); // Creamos el tablero adaptandose a la palabra objetivo
        CrearTeclado(); // Creamos el teclado con sus botones y su funcionalidad
    }
    public void CargarPalabra()
    {
        HttpClient cliente = new HttpClient();

        // .Result obliga a esperar la respuesta aquí mismo
        string json = cliente.GetStringAsync("https://rae-api.com/api/random?min_length=4&max_length=6").Result;

        // Procesamos el JSON
        JsonDocument documento = JsonDocument.Parse(json);
        string palabraExtraida = documento.RootElement.GetProperty("data").GetProperty("word").GetString();

        // Guardamos en tu propiedad
        PalabraSecreta = palabraExtraida.ToUpper();
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
            //TODO: Implementar la variabilidad del numero de columnas para que se adapte a la longitud de la palabra objetivo
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

    void CrearTeclado()
    {
        // 1. Definimos las filas el numero de filas que va a tener nuestro teclado
        string[] filasTeclado = new string[3];
        filasTeclado[0] = "QWERTYUIOP";
        filasTeclado[1] = "ASDFGHJKL";
        filasTeclado[2] = "ENTER,Z,X,C,V,B,N,M,<--";

        // Vamos a recorrer cada fila del teclado para crear los botones correspondientes
        foreach (string filaTexto in filasTeclado)
        {
            // 2. Creamos un contenedor horizontal para cada fila
            HorizontalStackLayout fila = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 2
            };

            // 3. Determinamos cómo separar las teclas de esta fila

            //Array que va a contener la teclas separadas
            string[] teclasSeparadas;

            // En caso de que las teclas estén separadas por comas, las separamos usando Split mediante la coma
            if (filaTexto.Contains(","))
            {
                // Se paramos las teclas usando la coma como separador
                teclasSeparadas = filaTexto.Split(',');
            }
            else // Si no hay comas, significa que las teclas están juntas, entonces las separamos letra por letra
            {
                // Adaptamos el array al largo de la filaTexto, para separar cada letra individualmente
                teclasSeparadas = new string[filaTexto.Length];

                // Despues sacamos el string al array
                for (int i = 0; i < filaTexto.Length; i++)
                {

                    teclasSeparadas[i] = filaTexto[i].ToString();

                }
            }

            
            // 4. Creamos un botón para cada tecla y lo agregamos a la fila
            foreach (string texto in teclasSeparadas) // Usamos string y no char porque algunas teclas son palabras como "ENTER" o "<--"
            {

                // Creamos el botón con su texto
                Button botonNuevo = new Button
                {
                    Text = texto, // Le pasamos el nombre del boton que hemos sacado
                    BackgroundColor = Colors.LightGray,
                    TextColor = Colors.Black,
                    CornerRadius = 5
                    
                };

                botonNuevo.Clicked += (s, e) => PresionarTecla(botonNuevo);
                // Añadimos el boton creado a la fila 
                fila.Add(botonNuevo);

            }

            KeyboardLayout.Add(fila); // Añadimos la fila completa al diseño del teclado

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

            // 4. Animación simple simulando un rebote al escribir la leta
            // Lo ponemos await para que no se ejecute la 2 animacions a la vez y se ejecute en orden para que no se solapen
            await borderActual.ScaleTo(1.1, 50); // Crece un 10%
            await borderActual.ScaleTo(1.0, 50); // Vuelve a su tamaño

            // 5. A la palabra actual le añadimos la letra que se ha pulsado para ir formando la palabra que se va a ir escribiendo
            IntentoPalabraActual = IntentoPalabraActual + teclaPulsada;
            ColActual = ColActual + 1; // Cada vez que escribimos una leta le sumamo uno a las propieda de columna actual para la proxima letra que se escriba
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

    private async Task ValidarPalabra()
    {
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
                border.BackgroundColor = Color.FromArgb("#3a3a3c");
                border.Stroke = Color.FromArgb("#3a3a3c");
            }

            // Cambiamos el color de todos
            label.TextColor = Colors.White;

            //ANIMACIÓN DE GIRO 
            await border.RotateYTo(0, 150);
        }

        // 2. Comprobar resultado final usando las propiedades y actualizando el Label de la interfaz
        if (IntentoPalabraActual == PalabraSecreta)
        {
            // En lugar de DisplayAlert, usamos el Label de estado
            LabelMensaje.Text = "¡ENHORABUENA! HAS ACERTADO";
            LabelMensaje.TextColor = Colors.Green;

            // Deshabilitamos el juego de alguna forma si quieres, por ejemplo no permitiendo más filas
            FilaActual = 7;
        }
        else
        {
            // Avanzamos fila y reseteamos columnas/intento
            FilaActual = FilaActual + 1;
            ColActual = 0;
            IntentoPalabraActual = "";

            if (FilaActual == 6)
            {
                LabelMensaje.Text = "FIN DEL JUEGO. LA PALABRA ERA: " + PalabraSecreta;
                LabelMensaje.TextColor = Colors.Red;
            }
            else
            {
                LabelMensaje.Text = "Sigue intentándolo...";
                LabelMensaje.TextColor = Colors.Gray;
            }
        }
    }
}