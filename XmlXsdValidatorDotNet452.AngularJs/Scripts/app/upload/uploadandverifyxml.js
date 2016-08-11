(function () {
    'use strict';

    angular.module("xmlXsdValidator")
        .service("uploadAndVerifyXml", ["$window", 'Upload', 'apiUrl', function (window, Upload, apiUrl) {

            var self = this;
            self.title = 'UploadCtrl';
            self.xml = xml;
            self.files = [];

            function UploadFiles(files, xml) {

                return Upload.upload({
                    url: apiUrl + '?xml=' + xml,
                    data: { file: files }
                })
                .then(function (response) {
                    return response;
                }, function (err) {
                    console.log("Error status: " + err.status);
                    console.log(err, err);
                    return err;
                });
            }

            return {
                UploadFiles: UploadFiles
            }
        }]);
})();