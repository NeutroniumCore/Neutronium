/// <reference path="../src/Infra.js" />
/// <reference path="../src/knockout.js" />
/// <reference path="../src/Ko_Extension.js" />
/// <reference path="../src/Koaddon.js" />


describe("Koaddon shoul map ok", function () {
    var basicmaped;

    beforeEach(function() {
        basicmaped = { Name: "Albert", LastName: "Einstein" };
    });

    it("should map basic property", function () {
        var mapped = ko.MapToObservable(basicmaped);
        ko.register(mapped);

        expect(mapped).not.toBeNull();
        expect(mapped.Name()).toBe("Albert");
        expect(mapped.LastName()).toBe("Einstein");
        expect(mapped.completeName()).toBe("Albert Einstein");
    });

    
});