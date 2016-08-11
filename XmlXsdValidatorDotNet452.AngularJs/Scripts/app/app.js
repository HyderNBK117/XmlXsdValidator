(function () {
    'use strict';

    angular.module('xmlXsdValidator', ['ngFileUpload', 'ngRoute'])
        .constant('apiUrl', 'api/Files/Post')
        .controller('HomeController', ['uploadAndVerifyXml', '$scope', function (uploadAndVerifyXml, $scope) {

            var self = this;

            self.files = [];
            self.xml = '';
            self.response = {
                message: '',
                errors: []
            };

            self.uploadFiles = function (files, xml) {

                uploadAndVerifyXml.UploadFiles(files, xml)
                    .then(function (response) {

                        self.files.length = 0;
                        self.response.message = response.data.m_Item1.toString();

                        while (self.response.errors.length) {
                            self.response.errors.pop();
                        }

                        self.response.errors.length = 0;

                        for (var i = 0; i < response.data.m_Item2.length; i++) {
                            self.response.errors.push(response.data.m_Item2[i]);
                        }
                    },
                    function (err) {
                        console.log(err);
                        self.files.length = 0;
                        self.response.message = err.data.Message;
                    });
            }

            self.beautifyXml = function () {
                self.xml = vkbeautify.xml(self.xml);
            }

            self.removeXsdFile = function (file) {
                self.files.pop(file);
            }
        }]);
})();