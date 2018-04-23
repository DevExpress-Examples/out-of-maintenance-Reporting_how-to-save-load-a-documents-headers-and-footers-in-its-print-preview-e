Imports Microsoft.VisualBasic
#Region "Usings"
Imports System
Imports System.IO
Imports System.Windows.Forms
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraPrinting.Preview
#End Region

Namespace SaveRestoreHeaderFooter
	Partial Public Class Form1
		Inherits Form
		Public Sub New()
			InitializeComponent()
		End Sub

		#Region "Internal Fields"
		Private Enum HeaderFooterStorage
			Registry
			XML
			Stream
		End Enum
		Private registryPath As String = "HKEY_CURRENT_USER\Software\MyCompany\MyTool\"
		Private xmlFile As String = "test.xml"
		Private stream As New MemoryStream()
		#End Region

		#Region "Prepare Example"
		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			Me.productsTableAdapter.Fill(Me.nwindDataSet.Products)

			radioGroup1.SelectedIndex = 0

			Dim phf As PageHeaderFooter = TryCast(printableComponentLink1.PageHeaderFooter, PageHeaderFooter)
			phf.Header.Content.AddRange(New String() { "", "Change Page Header/Footer, then close and re-open the form.", "" })

			SavePageHeaderFooter(printableComponentLink1, HeaderFooterStorage.Registry)
			SavePageHeaderFooter(printableComponentLink1, HeaderFooterStorage.Stream)
			SavePageHeaderFooter(printableComponentLink1, HeaderFooterStorage.XML)
		End Sub
		#End Region

		#Region "Get Header/Footer Storage"
		Private Function GetStorage() As HeaderFooterStorage
			Dim storage As HeaderFooterStorage = HeaderFooterStorage.Registry

			Select Case radioGroup1.SelectedIndex
				Case 0
						storage = HeaderFooterStorage.Registry
						Exit Select
				Case 1
						storage = HeaderFooterStorage.XML
						Exit Select
				Case 2
						storage = HeaderFooterStorage.Stream
						Exit Select
			End Select

			Return storage
		End Function
		#End Region

		Private Sub btnShowPreview_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnShowPreview.Click
			RestorePageHeaderFooter(printableComponentLink1, GetStorage())

			printableComponentLink1.CreateDocument()

			AddHandler printableComponentLink1.PrintingSystem.PreviewFormEx.FormClosed, AddressOf PreviewFormEx_FormClosed

			printableComponentLink1.ShowPreview()
		End Sub

		Private Sub PreviewFormEx_FormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs)
			SavePageHeaderFooter(printableComponentLink1, GetStorage())
		End Sub

		Private Sub RestorePageHeaderFooter(ByVal pcl As PrintableComponentLink, ByVal storage As HeaderFooterStorage)
			pcl.PageHeaderFooter = New PageHeaderFooter()
			Select Case storage
				Case HeaderFooterStorage.Registry
						pcl.RestorePageHeaderFooterFromRegistry(registryPath)
						Exit Select
				Case HeaderFooterStorage.XML
						If File.Exists(xmlFile) Then
							pcl.RestorePageHeaderFooterFromXml(xmlFile)
						End If
						Exit Select
				Case HeaderFooterStorage.Stream
						pcl.RestorePageHeaderFooterFromStream(stream)
						stream.Seek(0, SeekOrigin.Begin)
						Exit Select
			End Select
		End Sub

		Private Sub SavePageHeaderFooter(ByVal pcl As PrintableComponentLink, ByVal storage As HeaderFooterStorage)
			Select Case storage
				Case HeaderFooterStorage.Registry
						pcl.SavePageHeaderFooterToRegistry(registryPath)
						Exit Select
				Case HeaderFooterStorage.XML
						pcl.SavePageHeaderFooterToXml(xmlFile)
						Exit Select
				Case HeaderFooterStorage.Stream
						pcl.SavePageHeaderFooterToStream(stream)
						stream.Seek(0, SeekOrigin.Begin)
						Exit Select
			End Select
		End Sub

	End Class
End Namespace