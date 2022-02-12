(function () {

    var DXUploadedFilesContainer = {
        nameCellStyle: "",
        sizeCellStyle: "",
        thumbCellStyle: "",
        useExtendedPopup: false,

        AddFile: function (fileName, fileUrl, fileSize) {
            var self = DXUploadedFilesContainer;
            var builder = ["<tr>"];

            builder.push("<td class='nameCell'");
            if (self.nameCellStyle)
                builder.push(" style='" + self.nameCellStyle + "'");
            builder.push(">");
            self.BuildLink(builder, fileName, fileUrl);
            builder.push("</td>");

            builder.push("<td class='sizeCell'");
            if (self.sizeCellStyle)
                builder.push(" style='" + self.sizeCellStyle + "'");
            builder.push(">");
            builder.push(fileSize);
            builder.push("</td>");

            builder.push("</tr>");

            var html = builder.join("");
            DXUploadedFilesContainer.AddHtml(html);
        },
        AddThumbnail: function (imgUrl, title) {
            var self = DXUploadedFilesContainer;
            var builder = ["<tr>"];
            builder.push("<td class='imgCell' style='background:url(");
            builder.push(imgUrl);
            builder.push(") no-repeat;background-size:cover;height:100px;");
            if (self.thumbCellStyle)
                builder.push(" " + self.thumbCellStyle);
            builder.push("'>");
            builder.push("</td>");
            builder.push("<td class='nameCell' style='padding-left:8px;");
            if (self.nameCellStyle)
                builder.push(" " + self.nameCellStyle);
            builder.push("'>");
            self.BuildLink(builder, title, imgUrl);
            builder.push("</td>");
            builder.push("</tr>");
            var html = builder.join("");
            DXUploadedFilesContainer.AddHtml(html);
        },
        Clear: function () {
            DXUploadedFilesContainer.ReplaceHtml("");
        },
        BuildLink: function (builder, text, url) {
            builder.push("<a target='blank' onclick='return DXDemo.ShowScreenshotWindow(event, this, " + this.useExtendedPopup + ");'");
            builder.push(" href='" + url + "'>");
            builder.push(text);
            builder.push("</a>");
        },
        AddHtml: function (html) {
            var fileContainer = document.getElementById("uploadedFilesContainer"),
                fullHtml = html;
            if (fileContainer) {
                var containerBody = fileContainer.tBodies[0];
                fullHtml = containerBody.innerHTML + html;
            }
            DXUploadedFilesContainer.ReplaceHtml(fullHtml);
        },
        ReplaceHtml: function (html) {
            var builder = ["<table id='uploadedFilesContainer' class='uploadedFilesContainer'><tbody>"];
            builder.push(html);
            builder.push("</tbody></table>");
            var contentHtml = builder.join("");
            FilesRoundPanel.SetContentHtml(contentHtml);
        },
        ApplySettings: function (nameCellStyle, sizeCellStyle, thumbCellStyle, useExtendedPopup) {
            var self = DXUploadedFilesContainer;
            self.nameCellStyle = nameCellStyle;
            self.sizeCellStyle = sizeCellStyle;
            self.thumbCellStyle = thumbCellStyle;
            self.useExtendedPopup = useExtendedPopup;
        }
    };
    window.DXUploadedFilesContainer = DXUploadedFilesContainer;
})();

function onFileUploadComplete(e) {

    var productPath = $("#products-path").val();
    console.log(productPath);

    window.location.href = '/Product/Edit/' + $("#current-prod-id").val();

    if (e.callbackData) {
        var fileData = e.callbackData.split('|');
        var fileName = fileData[0],
            fileUrl = fileData[1],
            fileSize = fileData[2];
    }
}