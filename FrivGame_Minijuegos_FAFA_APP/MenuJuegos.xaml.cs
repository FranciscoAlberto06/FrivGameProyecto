using Microsoft.Maui.Controls.Shapes;

namespace FrivGame_Minijuegos_FAFA_APP;

public partial class MenuJuegos : ContentPage
{
	public MenuJuegos()
	{
		InitializeComponent();
        CrearJuegos();
    }

    private void CrearJuegos()
    {
        // Definimos los datos de forma explícita
        string[] titulos = { "TOPOS", "WORDLE", "PAREJAS" };
        string[] imagenes = { "topo.png", "wordle.png", "parejas.png" };
        Color[] colores = { Colors.Crimson, Colors.MediumSpringGreen, Colors.DeepSkyBlue };

        for (int i = 0; i < titulos.Length; i++)
        {
            Border contenedor = new Border
            {
                Stroke = colores[i],
                StrokeThickness = 3,
                StrokeShape = new RoundRectangle { CornerRadius = 15 },
                HeightRequest = 250,
                WidthRequest = 250,
                BackgroundColor = Colors.Black,
                Padding = 0
            };

            // Creamos el contenido (Imagen + Nombre)
            Grid contenidoInterno = new Grid();

            Image foto = new Image
            {
                Source = imagenes[i],
                Aspect = Aspect.AspectFill,
                Opacity = 0.9
            };

            Label nombre = new Label
            {
                Text = titulos[i],
                TextColor = Colors.White,
                FontSize = 28,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Opacity = 0 // Oculto al principio
            };

            contenidoInterno.Children.Add(foto);
            contenidoInterno.Children.Add(nombre);
            contenedor.Content = contenidoInterno;

            // Gestores de ratón
            PointerGestureRecognizer mouseGestura = new PointerGestureRecognizer();

            // Usamos una variable local para que el evento capture el objeto correcto
            Border actual = contenedor;
            Image actualFoto = foto;
            Label actualLabel = nombre;

            mouseGestura.PointerEntered += (s, e) => {
                actual.ZIndex = 10;
                actual.ScaleTo(1.2, 100);
                actualFoto.FadeTo(0.2, 100);
                actualLabel.FadeTo(1, 100);
            };

            mouseGestura.PointerExited += (s, e) => {
                actual.ZIndex = 0;
                actual.ScaleTo(1.0, 100);
                actualFoto.FadeTo(0.9, 100);
                actualLabel.FadeTo(0, 100);
            };

            contenedor.GestureRecognizers.Add(mouseGestura);

            // Añadir al Grid en la columna correspondiente
            MenuGrid.Children.Add(contenedor);
            Grid.SetColumn(contenedor, i);
        }
    }

}