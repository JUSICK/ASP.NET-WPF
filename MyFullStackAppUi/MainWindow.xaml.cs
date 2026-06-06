using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MyFullStackAppUi;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class AppUi : Window
{
    private readonly HttpClient _httpClient = new  HttpClient();
    
    private string _jwtToken = string.Empty;
    
    private const string ApiBaseUrl = "http://localhost:5027/api";
    
    public AppUi()
    {
        InitializeComponent();
    }

    private async void BtnRegister_Click(object sender, RoutedEventArgs e)
    {
        var requestData = new 
        { 
            Username = txtRegUser.Text, 
            Email = txtRegEmail.Text, 
            Password = txtRegPass.Password 
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/register", requestData);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Rejestracja zakończona sukcesem! Możesz się zalogować.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                txtRegUser.Text = ""; txtRegEmail.Text = ""; txtRegPass.Password = "";
            }
            else MessageBox.Show($"Błąd rejestracji. Kod: {response.StatusCode}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            
        }
        catch (HttpRequestException)
        {
            MessageBox.Show("Nie można połączyć się z serwerem. Czy API jest uruchomione?", "Błąd połączenia", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private async void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        var requestData = new 
        { 
            Email = txtLogEmail.Text, 
            Password = txtLogPass.Password 
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/login", requestData);

            if (response.IsSuccessStatusCode)
            { 
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    _jwtToken = result.Token; 
                    txtTokenDisplay.Text = _jwtToken; 
                    
                    MessageBox.Show("Zalogowano pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Błędny e-mail lub hasło.", "Odmowa dostępu", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (HttpRequestException)
        {
            MessageBox.Show("Nie można połączyć się z serwerem.", "Błąd połączenia", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private async void BtnFetchData_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_jwtToken))
        {
            MessageBox.Show("Brak tokena! Musisz się najpierw zalogować.", "Brak autoryzacji", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/user");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UserDataResponse>();
                lblServerResponse.Text = result?.Email ?? "Brak danych";
                lblServerResponseMessage.Text = result?.Message ?? "";
                lblServerResponse.Foreground = Brushes.Green;
                lblServerResponseMessage.Foreground = Brushes.Green;
            }
            else
            {
                lblServerResponse.Text = $"Błąd dostępu ({response.StatusCode})";
                lblServerResponse.Foreground = Brushes.Red;
            }
        }
        catch (HttpRequestException)
        {
            lblServerResponse.Text = "Błąd sieciowy";
            lblServerResponse.Foreground = Brushes.Red;
        }
    }
    
    
    private record AuthResponse(string Token);
    private record UserDataResponse(string Email, string Message);

}