(function () {
    function showOnOverlay(message) {
        const iframe = document.getElementById("neutronium-loading-overlay");
        iframe.updateDisplay(message);
    }

    showOnOverlay({information});
}())