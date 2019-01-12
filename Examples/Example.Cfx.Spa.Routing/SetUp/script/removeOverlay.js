(function () {
    function removeOverlay() {
        const iframe = document.getElementById("neutronium-loading-overlay");
        if (!iframe){
            return;
        }
        iframe.remove();
    }

    removeOverlay();
}())