window.printReceipt = function (url) {
    alert(url);
    const iframe = document.createElement("iframe");
    iframe.style.position = "fixed";
    iframe.style.right = "0";
    iframe.style.bottom = "0";
    iframe.style.width = "0";
    iframe.style.height = "0";
    iframe.style.border = "0";

    iframe.src = url;

    iframe.onload = function () {
        setTimeout(() => {
            iframe.contentWindow.focus();
            iframe.contentWindow.print();

            // cleanup after print
            setTimeout(() => {
                document.body.removeChild(iframe);
            }, 1000);
        }, 300);
    };

    document.body.appendChild(iframe);
};
