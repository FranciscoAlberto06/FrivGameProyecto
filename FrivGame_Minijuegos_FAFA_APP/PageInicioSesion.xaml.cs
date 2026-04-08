

namespace FrivGame_Minijuegos_FAFA_APP;

public partial class PageInicioSesion : ContentPage
{
	public PageInicioSesion()
	{
		InitializeComponent();
		AnimacionInicio();
	}

    private async void AnimacionInicio()
    {
        // Escala inicial
        LogoImage.Scale = 0.8;

        // Aparece y hace el "bounce"
        await LogoImage.ScaleTo(1.2, 300); // sube rápido
        await LogoImage.ScaleTo(0.9, 200);   // baja un poco

        // Aquí hacemos el “pin” justo en el último rebote
        // TODO: Reproducir el sonido "pin.wav" aquí cuando bote el logo y investigar sobre el puglin necesario 
        //MediaPlayer player = MediaPlayer.Create(MediaSource.FromFile("pin.wav"));
        //player.Play();

        await LogoImage.ScaleTo(1.0, 150); // rebote final

        // Fade out del splash
        await SplashContainer.FadeTo(0, 800);
        SplashContainer.IsVisible = false;

        // Fade in del login
        await LoginContainer.FadeTo(1, 800);
    }

    private async void botonInico(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MenuJuegos());
    }
}