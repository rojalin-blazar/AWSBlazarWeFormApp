<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Bucket.aspx.cs" MasterPageFile="~/Site.Master" Inherits="AWSBlazarWeFormApp.Bucket" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
        <h2>Your contact page.</h2>
    </hgroup>
    <div>
    <asp:TextBox ID="txtCreateBucket" runat="server"></asp:TextBox>
        <asp:Button ID="btnCreateBucket" Text="Create Bucket" runat="server" OnClick="btnCreateBucket_Click" />
    </div>
    </asp:Content>
