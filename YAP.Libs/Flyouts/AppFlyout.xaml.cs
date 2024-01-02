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
                    FlyoutMenuPage menuFlyout = new FlyoutMenuPage(RootItem);
                    FlyoutPageItem obj = new FlyoutPageItem();

                    mainFlyout.Flyout = menuFlyout;

                    if (RootItem.SelectedMenuIndex == -1)
                    {
                        obj = RootItem.MenuItemList.FirstOrDefault();
                    }
                    else
                    {
                        obj = RootItem.MenuItemList[RootItem.SelectedMenuIndex];
                    }

                    //mainFlyout.Detail = new NavigationPage((Page)Activator.CreateInstance(obj.TargetType));
                    menuFlyout.collectionView.SelectionChanged += OnSelectionChanged;
                    menuFlyout.collectionView.SelectedItem = obj;
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