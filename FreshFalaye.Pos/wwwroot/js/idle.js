window.posIdle = {
    start: function (dotnetRef) {
        const events = ["mousemove", "keydown", "mousedown", "touchstart"];

        events.forEach(e =>
            document.addEventListener(e, () => {
                dotnetRef.invokeMethodAsync("OnUserActivity");
            })
        );
    }
};
