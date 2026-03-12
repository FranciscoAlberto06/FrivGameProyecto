namespace FrivGame_Minijuegos_FAFA_APP;

public class PaginaNuevaPrueba : ContentPage
{
	public PaginaNuevaPrueba()
	{
		Content = new VerticalStackLayout
		{
			Children = {
				new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
				}
			}
		};
	}
}