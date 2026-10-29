namespace CrmApp.Services;

public interface INavigationService
{
    Task GoToAsync(string route);
    Task GoBackAsync();
}