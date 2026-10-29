namespace CrmApp.ViewModels;

public interface ILifecycleAware
{
    void OnAppearing();
    void OnDisappearing();
}