//window.printReceipt = function (url) {
//    const iframe = document.createElement("iframe");
//    iframe.style.position = "fixed";
//    iframe.style.right = "0";
//    iframe.style.bottom = "0";
//    iframe.style.width = "0";
//    iframe.style.height = "0";
//    iframe.style.border = "0";

//    iframe.src = url;

//    iframe.onload = function () {
//        setTimeout(() => {
//            iframe.contentWindow.focus();
//            iframe.contentWindow.print();

//            // cleanup after print
//            setTimeout(() => {
//                document.body.removeChild(iframe);
//            }, 1500);
//        }, 500);
//    };

//    document.body.appendChild(iframe);
//};

window.printReceipt = function (url) {
    
    const iframe = document.createElement("iframe");

    iframe.style.position = "fixed";
    iframe.style.width = "0";
    iframe.style.height = "0";
    iframe.style.border = "0";

    iframe.src = "/"; // 🔥 THIS triggers Blazor routing

    iframe.onload = function () {
        setTimeout(() => {

            iframe.contentWindow.Blazor.navigateTo(url);

            iframe.contentWindow.focus();
            iframe.contentWindow.print();

            setTimeout(() => {
                document.body.removeChild(iframe);
            }, 1500);
        }, 500);
    };

    document.body.appendChild(iframe);
};



