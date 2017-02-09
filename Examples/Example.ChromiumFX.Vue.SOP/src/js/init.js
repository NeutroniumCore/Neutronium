(function () {
    var localMixin = {
        methods: {
            testSameOriginPolicy: function () {
                var ajaxRequest = $.ajax({
                    url: 'http://www.google.com'
                })
                .success(function () {
                    alert('Same Origin Policy Off');
                })
                .error(function (ajaxRequest) {
                    if (ajaxRequest.isRejected() && ajaxRequest.status === 0) {
                        alert('Same Origin Policy On');
                    }
                });
            }
        }
    };

    Vue._vmMixin = [localMixin];
}());




