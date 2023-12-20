using YAP.Libs.Models;

namespace YAP.Libs.Flyouts;

public partial class AppFlyout : FlyoutPage
{
    RootItem RootItem { get; set; }

    public AppFlyout(RootItem rootItem)
    {
        try
        {
            InitializeComponent();

            RootItem = rootItem;

            if (RootItem.MenuItemList != null)
            {
                if (RootItem.MenuItemList.Count > 0)
                {
                    var menuFlyout = new FlyoutMenuPage(RootItem);

                    mainFlyout.Flyout = menuFlyout;
                    mainFlyout.Detail = new NavigationPage((Page)Activator.CreateInstance(RootItem.MenuItemList.FirstOrDefault().TargetType));

                    menuFlyout.collectionView.SelectedItem = RootItem.MenuItemList.FirstOrDefault();
                    menuFlyout.collectionView.SelectionChanged += OnSelectionChanged;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(AppFlyout) + " - " + ex.Message);
        }
    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;
            if (item != null)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                IsPresented = false;
            }
        }
        catch (Exception ex)
        {
            Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(OnSelectionChanged) + " - " + ex.Message);
        }
    }
}