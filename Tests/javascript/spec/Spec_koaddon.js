
/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
/// <reference path="../../../MVVM.CEFGlue/Javascript/knockout.js" />
/// <reference path="../../../MVVM.CEFGlue/Javascript//Ko_Extension.js" />
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