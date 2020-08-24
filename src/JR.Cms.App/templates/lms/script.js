function $G(TID) {
    return document.getElementById(TID);
}

function ShowSubMenu(show_id, hide_ids) {
    if (hide_ids != "") {
        for (var i = 0; i < hide_ids.split(",").length; i++) {
            try {
                $G(hide_ids.split(",")[i]).style.display = 'none';
            } catch (e) {}
        }
    }

    try {
        $G(show_id).style.display = '';
    } catch (e) {}
}


function searchClick() {
    var s = $b.$fn(".mod-top-search-box").val();
    if (s != "") {
        var lang = location.pathname.substring(1);
        var i = lang.indexOf('/');
        if (i != -1) {
            lang = lang.substring(0, i);
        }
        location.assign("/" + lang + "/page/product_search.html?c=product&w=" + s);
    }
}


$b.event.add(document.body, "load", function() {
    $b.$fn(".btn-search-close").click(function() {
        $b.$fn(".mod-top-search").slideUp("slow");
    });
    $b.$fn(".btn-search-collage").click(function() {
        $b.$fn(".mod-top-search").slideDown("slow");
    });
    if ($b.$fn(".auto-slider").len() > 0) {
        $('.auto-slider').slick({
            slidesToShow: 1,
            slidesToScroll: 1,
            autoplay: true,
            autoplaySpeed: 2000,
        });
    }
});


function showBgImage(index) {
    try {
        $G("mainnav").getElementsByTagName("li")[index].className = "l1 here current";
    } catch (e) {}
}

// JScript 文件
function getCookie(sCookieName) {
    var sName = sCookieName + "=",
        ichSt, ichEnd;
    var sCookie = document.cookie;

    if (sCookie.length && (-1 != (ichSt = sCookie.indexOf(sName)))) {
        if (-1 == (ichEnd = sCookie.indexOf(";", ichSt + sName.length)))
            ichEnd = sCookie.length;
        return unescape(sCookie.substring(ichSt + sName.length, ichEnd));
    }

    return null;
}

function setCookie(sName, vValue) {
    var argv = setCookie.arguments,
        argc = setCookie.arguments.length;
    var sExpDate = (argc > 2) ? "; expires=" + argv[2].toGMTString() : "";
    var sPath = "; path=/"; // (argc > 3) ? "; path="+argv[3] : "";
    var sDomain = (argc > 4) ? "; domain=" + argv[4] : "";
    var sSecure = (argc > 5) && argv[5] ? "; secure" : "";
    document.cookie = sName + "=" + escape(vValue, 0) + sExpDate + sPath + sDomain + sSecure + ";";
}

function deleteCookie(sName) {
    document.cookie = sName + "=" + getCookie(sName) + "; expires=" + (new Date()).toGMTString() + ";";
}

//------------------------------------

function setProductInqurie(pid, ask) {
    var oldCookie = getCookie("InquireList") == null ? "" : getCookie("InquireList");
    if (pid.replace(" ", "") != "" && oldCookie.indexOf(pid + ",") < 0) {
        setCookie("InquireList", oldCookie + pid + ",");
    }
    if (ask == true) {
        if (window.confirm("Inquiry for the product was submitted successfully. Shift to the page of products inquired ?")) {
            location.href = "/en/service/inquirybasket";
        }
    }
}

function selectAll() {
    var cbs = document.getElementsByName("CheckBox_Prod");
    var check_ids = "";
    for (var i = 0; i < cbs.length; i++) {
        if (cbs[i].checked == true) {
            check_ids += cbs[i].value + ",";
            setProductInqurie(cbs[i].value, false);
        }
    }
    if (check_ids != "") {
        if (window.confirm("Inquiry for the product was submitted successfully. Shift to the page of products inquired ?")) {
            location.href = "/en/service/inquirybasket";
        }
    } else {
        alert("Please select products");
    }
    //
    //event.returnValue = false;
    return false;
}

function clickThisImg(Name) {

    $G("Img").src = Name;
}

function chgH3_Style(id) {
    try {
        $G(id).className = "here";
    } catch (e) {}
}

function chgH4_Style(id) {
    try {
        $G(id).className = "here";
    } catch (e) {}
}

function showContent(obj, a_id) {
    var ps = "p_Description,p_Specification,p_Picture,p_Case";
    for (var i = 0; i < ps.split(",").length; i++) {
        try {
            $G(ps.split(",")[i]).style.display = "none";
        } catch (e) {}
    }
    obj.style.display = "block";

    //
    var as = "a_show_Description,a_show_Specification,a_show_Picture,a_show_Case,"; //background:#EBF1F8; color:#2E3192;
    for (var i = 0; i < as.split(",").length; i++) {
        try {
            $G(as.split(",")[i]).style.background = "#7298CB";
            $G(as.split(",")[i]).style.color = "#FFFFFF";
        } catch (e) {}
    }
    $G(a_id).style.backgroundColor = "#EBF1F8";
    $G(a_id).style.color = "#2E3192";
    try {
        window.event.returnValue = false;
    } catch (e) {}
    return false;
}

function AddClick(ID, TYPE) {
    var url = "<iframe width=0 height=0 style='display:none' src='aspx/AddClick.aspx/TYPE=" + TYPE;
    url += "&IDentityID=";
    url += ID;
    url += "'></iframe>";
    document.write(url);
}

function AjaxGetPro_Next(ControlID, ObjectID, Type) {
    var xmlhttp;
    try {
        xmlhttp = new XMLHttpRequest();
    } catch (e) {
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }

    xmlhttp.onreadystatechange = function() {
        if (4 == xmlhttp.readyState) {
            if (200 == xmlhttp.status) {

                var value = xmlhttp.responseText;
                document.getElementById(ControlID).innerHTML = value;
            } else {
                return null;
            }
        }
    }
    xmlhttp.open("post", "/aspx/Ajax.aspx", true);
    xmlhttp.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
    xmlhttp.send("IdentityID=" + escape(ObjectID) + "&Type=" + escape(Type));
}

function AjaxGetRemmProd(ControlID, ObjectID) {
    var xmlhttp;
    try {
        xmlhttp = new XMLHttpRequest();
    } catch (e) {
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }

    xmlhttp.onreadystatechange = function() {
        if (4 == xmlhttp.readyState) {
            if (200 == xmlhttp.status) {

                var value = xmlhttp.responseText;
                document.getElementById(ControlID).innerHTML = value;
            } else {
                return null;
            }
        }
    }
    xmlhttp.open("post", "/aspx/Ajax.aspx", true);
    xmlhttp.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
    xmlhttp.send("GetREMMPROD=" + escape(ObjectID));
}


/*  newmin append on 2010.06.28 */
function removeSelectedProducts() {
    var removeProduct = "";
    var pros = document.getElementById("productslist").getElementsByTagName("input");
    for (var i in pros) {
        if (pros[i].type == "checkbox") {
            if (pros[i].checked) {
                pros[i].parentNode.parentNode.style.display = 'none';
                removeProduct += pros[i].value + "|";
            }
        }
    }
    if (removeProduct != "") {
        removeProduct = removeProduct.substring(0, removeProduct.length - 1);
        cms.xhr.post("/com.lms.sh.aspx/SaveInquiryBasket", "action=removeproduct&list=" + removeProduct, function() {}, function(x) {});
    }
}

function inquireAllProducts() {
    var pros = document.getElementById("productslist").childNodes;
    if (pros.length == 0) {
        alert("sorry,you has inquire 0 products!");
    } else {
        //location.href="inquirybasket?action=order";
        cms.$G('setup1').style.display = 'none';
        cms.$G('setup2').style.display = '';
    }
}