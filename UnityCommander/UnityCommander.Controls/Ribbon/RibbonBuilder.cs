
namespace UnityCommander.Controls.Ribbon
{
    /// <summary>
    /// The ribbon group builder.
    /// </summary>
    public class RibbonBuilder
    {
        /// <summary>
        /// The ribbon.
        /// </summary>
        private Ribbon ribbon;

        /// <summary>
        /// The section.
        /// </summary>
        private RibbonSectionBuilder sectionBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonBuilder"/> class.
        /// </summary>
        public RibbonBuilder()
        {
            this.ribbon = new Ribbon();
        }

        /// <summary>
        /// The add section.
        /// </summary>
        /// <param name="groupBuilder">
        /// The group Builder.
        /// </param>
        /// <param name="header">
        /// The tab tab header.
        /// </param>
        public void SetSection(RibbonGroupBuilder groupBuilder, string header)
        {
            //if (this.sectionBuilder != null)
            //{
            //    var ribbonSectionBuilder = this.sectionBuilder;
            //    if (ribbonSectionBuilder != null)
            //    {
            //        ribbonSectionBuilder.SetSection(groupBuilder, header);
            //    }
            //}
            //else
            //{
            //    this.sectionBuilder = new RibbonSectionBuilder(groupBuilder, header);
            //}
            
            //var content = this.sectionBuilder?.GetSectionGrid();
            //this.ribbon.Children.Clear();
            //this.ribbon.Children.Add(content);
        }

        /// <summary>
        /// The get section.
        /// </summary>
        /// <returns>
        /// The <see cref="Ribbon"/>.
        /// </returns>
        public Ribbon Build() => this.ribbon;
    }
}
