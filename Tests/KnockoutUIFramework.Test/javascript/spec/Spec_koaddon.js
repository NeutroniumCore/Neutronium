/// <reference path="../../../../knockoutuiframework/scripts/infra.js" />
/// <reference path="../../../../knockoutuiframework/scripts/knockout.js" />
/// <reference path="../../../../knockoutuiframework/scripts/ko_extension.js" />
/// <reference path="../../../../mvvm.html.core/binding/mapping/scripts/infra.js" />
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