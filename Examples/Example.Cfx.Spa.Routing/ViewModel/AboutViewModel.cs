namespace Example.Cfx.Spa.Routing.ViewModel {
    public class AboutViewModel {
        public ApplicationInformation Information { get; } = new ApplicationInformation();

        public string[] Descriptions { get; } =
        {
            Resource.About1,
            Resource.About2,
            Resource.About3
        };
    }
}
