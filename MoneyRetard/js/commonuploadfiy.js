var wait = 0;
function Commonuploadfiy(url, DivID, limitFile, fileID, saveFileID, outId) {
    $("#" + fileID).uploadify({
        'uploader': url,
        'swf': '../js/uploadfiy/uploadify.swf',
        'cancelImg': '../js/uploadfiy/img/uploadify-cancel.png',
        'sizeLimit': '300000000000',
        'queueID': DivID,
        'removeCompleted': false,
        'height': 20, //上传按钮的高和宽
        'width': 55,
        'auto': false,
        'multi': true,
        'buttonText': "添加附件",
        'fileTypeExts': limitFile, //允许上传的文件类型，限制弹出文件选择框里能选择的文件
        'queueSizeLimit': 5, //队列中允许的最大文件数目
        'onSelectError': uploadify_onSelectError,
        onError: function (a, b, c, d) {
            if (d.status == 404)
                alert('Could not find upload script. Use a path relative to: ' + '<?= getcwd() ?>');
            else if (d.type === "HTTP")
                alert('error ' + d.type + ": " + d.status);
            else if (d.type === "File Size")
                alert(c.name + ' ' + d.type + ' Limit: ' + Math.round(d.sizeLimit / 1024) + 'KB');
            else
                alert('error ' + d.type + ": " + d.info);
        },
        onUploadSuccess: function (file, data, response) {
            document.getElementById(saveFileID).value += data + "|";
            //            if (wait == 0) {
            //                $("#" + outId).hide();
            //            } else {
            //                setTimeout('TimeOutHide()',1000)
            //            }
            //alert(file.name)
            //            this.enable();
            //            var responseText = response.toString();
            //            alert(responseText)
            //            if (/MSIE 6.0/i.test(navigator.userAgent)) {
            //                responseText = getCookies();
            //            }
            //            document.getElementById("hidd_UploadFiles").value += param + "%" + file + "&";
            //            var imgCount = $("#hidd_imgCount").val();
            //            alert(document.getElementById("hidd_UploadFiles").value)
        },
        /*queueData对象包含如下属性：
        uploadsSuccessful – 上传成功的文件数量
        uploadsErrored – 上传失败的文件数量**/
        onQueueComplete: function (queueData) { //文件上传队列处理完毕后触发。
            $("#" + outId).hide();//弹出层的DIV隐藏
        }
    });
}
var uploadify_onSelectError = function (file, errorCode, errorMsg) {
    var msgText = "上传失败\n";
    switch (errorCode) {
        case SWFUpload.QUEUE_ERROR.QUEUE_LIMIT_EXCEEDED:
            //this.queueData.errorMsg = "每次最多上传 " + this.settings.queueSizeLimit + "个文件";  
            msgText += "每次最多上传 " + this.settings.queueSizeLimit + "个文件";
            break;
        case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
            msgText += "文件大小超过限制( " + this.settings.fileSizeLimit + " )";
            break;
        case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
            msgText += "文件大小为0";
            break;
        case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
            msgText += "文件格式不正确，仅限 " + this.settings.fileTypeExts;
            break;
        default:
            msgText += "错误代码：" + errorCode + "\n" + errorMsg;
    }
    alert(msgText);
};
function TimeOutHide() {
    wait--;
}
function sumbitonclick(fileID) {
    $('#'+fileID).uploadify('upload', '*');
}
function cancelonclick(fileID) {
    $('#' + fileID).uploadify('cancel', '*');
}