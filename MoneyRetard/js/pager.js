function getLink(pageIndex, totalpage, total, urls, TwoStatusDiv, pageLink) {
    linkHTML = '<span>' + pageIndex + '/' + totalpage + '页</span><a href="' + urls + '?pager=1">首页</a>&nbsp;&nbsp;';
    //var page=totalpage;
    var start_page = 0;
    var end_page = 0;
    var pageshownum = 0; //页面上显示最多几个数字页码
    if (pageIndex < pageshownum - 1) {
        start_page = 1;
        end_page = pageshownum;
    } else {
        start_page = pageIndex - (pageshownum - 2);
        end_page = pageIndex + 1;
    }
    if (end_page > totalpage) {
        end_page = totalpage;
    }
    if (pageIndex > 1) {
        linkHTML += '<a href="' + urls + '?pager=' + (pageIndex - 1) + '">上一页</a>&nbsp;&nbsp';
    }
    else {
        linkHTML += '上一页&nbsp;&nbsp';
    }
    for (var i = start_page; i <= end_page; i++) {

        if (pageIndex == i) {
            linkHTML += '<span class="current">' + i + '</span>&nbsp;&nbsp;'
        }
        else {
            linkHTML += '<a href=\"' + urls + '?pager=' + i + '\" >' + i + '</a>&nbsp;&nbsp;'
        }


    }
    if (pageIndex < totalpage) {
        linkHTML += '<a href=\"' + urls + '?pager=' + (pageIndex + 1) + '\">下一页</a>&nbsp;&nbsp';
    }
    else {
        linkHTML += '下一页&nbsp;&nbsp';
    }
    linkHTML += '<a href=\"' + urls + '?pager=' + totalpage + '\">末页</a> &nbsp;&nbsp;';
    $("#" + pageLink).html(linkHTML);
}