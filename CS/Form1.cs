#region Usings
using System;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Preview;
#endregion

namespace SaveRestoreHeaderFooter {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        #region Internal Fields
        private enum HeaderFooterStorage { Registry, XML, Stream };
        private string registryPath = @"HKEY_CURRENT_USER\Software\MyCompany\MyTool\";
        private string xmlFile = "test.xml";
        private MemoryStream stream = new MemoryStream();
        #endregion

        #region Prepare Example
        private void Form1_Load(object sender, EventArgs e) {
            this.productsTableAdapter.Fill(this.nwindDataSet.Products);

            radioGroup1.SelectedIndex = 0;

            PageHeaderFooter phf = printableComponentLink1.PageHeaderFooter as PageHeaderFooter;
            phf.Header.Content.AddRange(new string[] { "", 
                "Change Page Header/Footer, then close and re-open the form.", "" });

            SavePageHeaderFooter(printableComponentLink1, HeaderFooterStorage.Registry);
            SavePageHeaderFooter(printableComponentLink1, HeaderFooterStorage.Stream);
            SavePageHeaderFooter(printableComponentLink1, HeaderFooterStorage.XML);            
        }
        #endregion

        #region Get Header/Footer Storage
        private HeaderFooterStorage GetStorage() {
            HeaderFooterStorage storage = HeaderFooterStorage.Registry;

            switch (radioGroup1.SelectedIndex) {
                case 0: {
                        storage = HeaderFooterStorage.Registry;
                        break;
                    }
                case 1: {
                        storage = HeaderFooterStorage.XML;
                        break;
                    }
                case 2: {
                        storage = HeaderFooterStorage.Stream;
                        break;
                    }
            }

            return storage;
        }
        #endregion

        private void btnShowPreview_Click(object sender, EventArgs e) {
            RestorePageHeaderFooter(printableComponentLink1, GetStorage());

            printableComponentLink1.CreateDocument();

            printableComponentLink1.PrintingSystem.PreviewFormEx.FormClosed +=
                new FormClosedEventHandler(PreviewFormEx_FormClosed);

            printableComponentLink1.ShowPreview();
        }

        void PreviewFormEx_FormClosed(object sender, FormClosedEventArgs e) {
            SavePageHeaderFooter(printableComponentLink1, GetStorage());
        }

        private void RestorePageHeaderFooter(PrintableComponentLink pcl, HeaderFooterStorage storage) {
            pcl.PageHeaderFooter = new PageHeaderFooter();
            switch (storage) {
                case HeaderFooterStorage.Registry: {
                        pcl.RestorePageHeaderFooterFromRegistry(registryPath);
                        break;
                    }
                case HeaderFooterStorage.XML: {
                        if (File.Exists(xmlFile)) {
                            pcl.RestorePageHeaderFooterFromXml(xmlFile);
                        }
                        break;
                    }
                case HeaderFooterStorage.Stream: {
                        pcl.RestorePageHeaderFooterFromStream(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        break;
                    }
            }
        }

        private void SavePageHeaderFooter(PrintableComponentLink pcl, HeaderFooterStorage storage) {
            switch (storage) {
                case HeaderFooterStorage.Registry: {
                        pcl.SavePageHeaderFooterToRegistry(registryPath);
                        break;
                    }
                case HeaderFooterStorage.XML: {
                        pcl.SavePageHeaderFooterToXml(xmlFile);
                        break;
                    }
                case HeaderFooterStorage.Stream: {
                        pcl.SavePageHeaderFooterToStream(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        break;
                    }
            }
        }

    }
}