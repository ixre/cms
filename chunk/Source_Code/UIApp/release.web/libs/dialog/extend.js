if(cms){
    cms.ldScript(cms.libpath + 'dialog/zdialog.js');
    cms.ldScript(cms.libpath + 'dialog/zdrag.js');

    cms.dialog = {
        alert: function (msg, func) {
            Dialog.alert('<span style="color:#ff6600;font-size:14px;font-weight:bold">'+msg+'</span>', func);
        },
        confirm: function (msg, func) {
            Dialog.confirm(msg, function () { if (func) func(); });
        },
        open: function (url, width, height) {
            var diag = new Dialog();
            if (width) {
                diag.Width = width;
            }
            if (height) {
                diag.Height = height;
            }
            //diag.Title = "内容页为外部连接的窗口";
            diag.URL =url;
            diag.show();
            return diag;
        },
        html: function (html, width, height, okEvent, title, hidBtn) {
            var diag = new Dialog();
            if (width) {
                diag.Width = width;
            } if (height) {
                diag.Height = height;
            }
            if (title) {
                diag.Title = title;
            }
            if (hidBtn) {
                diag.ShowButtonRow = false;
            } else {
                diag.OKEvent = function () { if (okEvent) okEvent(); else { diag.close(); } }; //点击确定后调用的方法
            }
            diag.InnerHtml = '<div style="text-align:center;">' + html + '</div>'
            diag.show();
            return diag;
        }
    };
}