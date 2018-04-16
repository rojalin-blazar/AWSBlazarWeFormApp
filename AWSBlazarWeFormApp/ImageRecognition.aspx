<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImageRecognition.aspx.cs" MasterPageFile="~/Site.Master" Inherits="AWSBlazarWeFormApp.ImageRecognition" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %></h1>
        <h3>Upload student Image</h3>
    </hgroup>
<div>
    <%--<asp:Label ID="lblUpload" Text="Please upload your photo" runat="server"></asp:Label>--%>
        <asp:FileUpload ID="fuImage" runat="server" />
        <asp:Button ID="btnUpload" Text="Upload Image" runat="server" CssClass="button"  OnClick="btnUpload_Click"/>
    <asp:Button Text="AddImageToCollection" ID="btnAddToCollection" Visible="false" runat="server" OnClick="btnAddToCollection_Click"/>
   
    </div>
    <div>
        <asp:Label ID="lblMessage" Text="" runat="server" Font-Bold="true"></asp:Label>
        
    </div>
    <br />
    <div id="image"  visible="false" runat="server"> 
       Matching Image: <img id="existingImage"   runat="server" height="200" width="200" />
       Uploaded Image: <img id="dupImage"  runat="server" height="200" width="200" />
    </div>
</asp:Content>

