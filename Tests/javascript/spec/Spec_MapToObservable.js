/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
/// <reference path="../../../MVVM.CEFGlue/Javascript/knockout.js" />
/// <reference path="../../../MVVM.CEFGlue/Javascript//Ko_Extension.js" />


describe("Map To Observable", function () {
    var basicmaped, basicmaped2, basicmaped3, basicmaped4,
        basicmaped5, basicmaped6, basicmapped7, mapwithenum;


    beforeEach(function() {
        basicmaped = { Name: "Albert", LastName: "Einstein", Age:55 };
        basicmaped2 = { Name: "Mickey", LastName: "Mouse" };
        basicmaped3 = { One: basicmaped2, Two: basicmaped2 };
        basicmaped4 = { One: basicmaped, Two: basicmaped2 };
        basicmaped5 = { List: ['un', 'deux', 'trois'] };
        basicmaped6 = { List: [{ Name: '1' }, { Name: '2' }, { Name: '3' }] };
        basicmapped7 = { When: new Date(1974, 2, 26) };
        mapwithenum = { Enum: new Enum(0, '34') };
    });

    it("should map basic property", function () {
        var mapped = ko.MapToObservable(basicmaped);

        expect(mapped).not.toBeNull();
        expect(mapped.Name()).toBe("Albert");
        expect(mapped.LastName()).toBe("Einstein");
    });

    it("should map collection", function () {
        var mapped = ko.MapToObservable(basicmaped5);

        expect(mapped).not.toBeNull();
        var list = mapped.List()();
        expect(list).not.toBeNull();
        expect(list.length).toBe(3);
        expect(list).toContain("un");
        expect(list).toContain("deux");
        expect(list).toContain("trois");
    });

    it("should preserve references", function () {
        var mapped = ko.MapToObservable(basicmaped3);
        var mapped2 = ko.MapToObservable(basicmaped2);

        expect(mapped.One()).toBe(mapped.Two());
        expect(mapped.One()).toBe(mapped2);
    });

    it("should use caching", function () {
        var mapped = ko.MapToObservable(basicmaped2);
        var mapped2 = ko.MapToObservable(basicmaped2);

        expect(mapped).toBe(mapped2);
    });

    it("should work with nested", function () {
        var mapped = ko.MapToObservable(basicmaped4);

        expect(mapped.One().Name()).toBe("Albert");
        expect(mapped.Two().Name()).toBe("Mickey");
    });


    it("should implement silent with nested", function () {
        var mapped = ko.MapToObservable(basicmaped4);

        expect(mapped.One.silent).not.toBe(undefined);
        expect(typeof mapped.One.silent).toBe("function");
    });


    it("should call the mapper register", function () {
        var mapper = { Register: function () { } };

        spyOn(mapper, 'Register');

        var mapped = ko.MapToObservable(basicmaped, mapper);

        expect(mapper.Register).toHaveBeenCalled();
        expect(mapper.Register).toHaveBeenCalledWith(mapped);
        expect(mapper.Register.calls.count()).toEqual(1);
    });

    it("should call the mapper register with good parameters: nested", function () {
        var mapper = { Register: function () { } };

        spyOn(mapper, 'Register');

        var mapped = ko.MapToObservable(basicmaped4, mapper);
        var mapped_One = ko.MapToObservable(basicmaped4.One, mapper);
        var mapped_Two = ko.MapToObservable(basicmaped4.Two, mapper);

        expect(mapper.Register).toHaveBeenCalled();
        expect(mapper.Register).toHaveBeenCalledWith(mapped);
        expect(mapper.Register).toHaveBeenCalledWith(mapped_One,mapped,  'One' );
        expect(mapper.Register).toHaveBeenCalledWith(mapped_Two, mapped, 'Two' );

        expect(mapper.Register.calls.count()).toEqual(3);
    });

    it("should call the mapper register with good parameters: nested and shared", function () {
        var mapper = { Register: function () { } };
        spyOn(mapper, 'Register');
        var mapped = ko.MapToObservable(basicmaped3, mapper);

        var mapped_One = ko.MapToObservable(basicmaped3.One);
  
        expect(mapper.Register).toHaveBeenCalled();
        expect(mapper.Register).toHaveBeenCalledWith(mapped);
        expect(mapper.Register).toHaveBeenCalledWith(mapped_One,  mapped, 'One' );
  
        expect(mapper.Register.calls.count()).toEqual(2);
    });

    it("should call the mapper register with good parameters: Collection", function () {
        var mapper = { Register: function () { } };
        spyOn(mapper, 'Register');
        var mapped = ko.MapToObservable(basicmaped6, mapper);

        var mapped_One = ko.MapToObservable(basicmaped6.List[0]);
        var mapped_Two = ko.MapToObservable(basicmaped6.List[1]);
        var mapped_Three = ko.MapToObservable(basicmaped6.List[2]);

        expect(mapped.List()()[0]).toBe(mapped_One);
        expect(mapped.List()()[1]).toBe(mapped_Two);
        expect(mapped.List()()[2]).toBe(mapped_Three);

        expect(mapper.Register).toHaveBeenCalled();
        expect(mapper.Register).toHaveBeenCalledWith(mapped);
        expect(mapper.Register).toHaveBeenCalledWith(mapped_One,  mapped, 'List', 0 );
        expect(mapper.Register).toHaveBeenCalledWith(mapped_Two,  mapped, 'List',  1 );
        expect(mapper.Register).toHaveBeenCalledWith(mapped_Three,  mapped, 'List',  2 );
        expect(mapper.Register).toHaveBeenCalledWith(mapped.List(), mapped,  'List');


        expect(mapper.Register.calls.count()).toEqual(5);
    });


    it("should call not register when object is cached", function () {
        var mapped = ko.MapToObservable(basicmaped);

        var mapper = { Register: function () { } };
        spyOn(mapper, 'Register');

        var mapped2 = ko.MapToObservable(basicmaped, mapper);

        expect(mapper.Register).not.toHaveBeenCalled();
    });


     it("should call the mapper End", function () {
        var mapper = { End: function () { } };

        spyOn(mapper, 'End');

        var mapped = ko.MapToObservable(basicmaped, mapper);

        expect(mapper.End).toHaveBeenCalled();
        expect(mapper.End.calls.count()).toEqual(1);
        expect(mapper.End).toHaveBeenCalledWith(mapped);
     });

     it("should call the mapper End even when cached", function () {
         var mapped0 = ko.MapToObservable(basicmaped);

         var mapper = { End: function () { } };
         spyOn(mapper, 'End');

         var mapped1 = ko.MapToObservable(basicmaped, mapper);

         expect(mapper.End).toHaveBeenCalled();
         expect(mapper.End.calls.count()).toEqual(1);
         expect(mapper.End).toHaveBeenCalledWith(mapped0);
     });

     it("should listen TrackChanges on string", function () {
         var Listener = { TrackChanges: function () { } };
         spyOn(Listener, 'TrackChanges');

         var mapped = ko.MapToObservable(basicmaped, null, Listener);

         mapped.Name("Toto");
         
         expect(mapped.Name()).toEqual("Toto");
         expect(Listener.TrackChanges).toHaveBeenCalled();
         expect(Listener.TrackChanges.calls.count()).toEqual(1);
         expect(Listener.TrackChanges).toHaveBeenCalledWith(mapped,'Name','Toto');
     });

   

     it("should listen TrackChanges on int", function () {
         var Listener = { TrackChanges: function () { } };
         spyOn(Listener, 'TrackChanges');

         var mapped = ko.MapToObservable(basicmaped, null, Listener);

         mapped.Age(10);

         expect(mapped.Age()).toEqual(10);
         expect(Listener.TrackChanges).toHaveBeenCalled();
         expect(Listener.TrackChanges.calls.count()).toEqual(1);
         expect(Listener.TrackChanges).toHaveBeenCalledWith(mapped, 'Age', 10);
     });

     it("should not listen TrackChanges on int when silent", function () {
         var Listener = { TrackChanges: function () { } };
         spyOn(Listener, 'TrackChanges');

         var mapped = ko.MapToObservable(basicmaped, null, Listener);

         mapped.Age.silent(10);

         expect(mapped.Age()).toEqual(10);
         expect(Listener.TrackChanges).not.toHaveBeenCalled();
         expect(Listener.TrackChanges.calls.count()).toEqual(0);
     });

     it("should re-listen TrackChanges on int when silent and then not", function () {
         var Listener = { TrackChanges: function () { } };
         spyOn(Listener, 'TrackChanges');

         var mapped = ko.MapToObservable(basicmaped, null, Listener);

         mapped.Age.silent(10);

         expect(mapped.Age()).toEqual(10);
         expect(Listener.TrackChanges).not.toHaveBeenCalled();
         expect(Listener.TrackChanges.calls.count()).toEqual(0);

         mapped.Age(60);

         expect(mapped.Age()).toEqual(60);
         expect(Listener.TrackChanges).toHaveBeenCalled();
         expect(Listener.TrackChanges.calls.count()).toEqual(1);
         expect(Listener.TrackChanges).toHaveBeenCalledWith(mapped, 'Age', 60);
     });

     it("should listen TrackChanges on Date", function () {
         var Listener = { TrackChanges: function () { } };
         spyOn(Listener, 'TrackChanges');

         var original = new Date(1974, 2, 26);
         var newDate = new Date(Date.now());

         var mapped = ko.MapToObservable(basicmapped7, null, Listener);

         expect(mapped.When()).toEqual(original);

         mapped.When(newDate);

       
         expect(Listener.TrackChanges).toHaveBeenCalled();
         expect(Listener.TrackChanges.calls.count()).toEqual(1);
         expect(Listener.TrackChanges).toHaveBeenCalledWith(mapped, 'When', newDate);
     });

     it("should define silent when no TrackChanges defined - date case", function () {
         var original = new Date(1974, 2, 26);
         var newDate = new Date(Date.now());

         var mapped = ko.MapToObservable(basicmapped7, null, null);

         expect(mapped.When()).toEqual(original);

         mapped.When.silent(newDate);
         expect(mapped.When()).toEqual(newDate);
     });

     it("should define silent when no TrackChanges defined - string case", function () {

         var mapped = ko.MapToObservable(basicmaped, null, null);

         mapped.Name.silent("Toto");

         expect(mapped.Name()).toEqual("Toto");
     });

     it("should not listen TrackChanges on Date when silent", function () {
         var Listener = { TrackChanges: function () { } };
         spyOn(Listener, 'TrackChanges');

         var original = new Date(1974, 2, 26);
         var newDate = new Date(Date.now());

         var mapped = ko.MapToObservable(basicmapped7, null, Listener);

         expect(mapped.When()).toEqual(original);

         mapped.When.silent(newDate);

         expect(mapped.When()).toEqual(newDate);
         expect(Listener.TrackChanges).not.toHaveBeenCalled();
         expect(Listener.TrackChanges.calls.count()).toEqual(0);

         newDate = new Date(2002,2,2);
 
         mapped.When(newDate);

         expect(mapped.When()).toEqual(newDate);
         expect(Listener.TrackChanges).toHaveBeenCalled();
         expect(Listener.TrackChanges.calls.count()).toEqual(1);
         expect(Listener.TrackChanges).toHaveBeenCalledWith(mapped, 'When', newDate);
     });

     it("should map Enum", function () {
         var Listener = { TrackChanges: function () { } };
         spyOn(Listener, 'TrackChanges');

         var original = mapwithenum.Enum;
         var newEnum = new Enum(1,'One');

         var mapped = ko.MapToObservable(mapwithenum, null, Listener);

         expect(mapped.Enum()).toEqual(original);

         mapped.Enum(newEnum);


         expect(Listener.TrackChanges).toHaveBeenCalled();
         expect(Listener.TrackChanges.calls.count()).toEqual(1);
         expect(Listener.TrackChanges).toHaveBeenCalledWith(mapped, 'Enum', newEnum);
     });

     

     it("should listen nested TrackChanges", function () {
         var Listener = {
             TrackChanges: function (who, property, value)
             { console.log('who :'+who+' property '+property+'value'+value); }
         };
          spyOn(Listener, 'TrackChanges').and.callThrough();;

         var mapped = ko.MapToObservable(basicmaped4, null, Listener);

         mapped.One().Name("Titi");

         expect(mapped.One().Name()).toEqual("Titi");
         expect(Listener.TrackChanges).toHaveBeenCalled();
         expect(Listener.TrackChanges.calls.count()).toEqual(1);
         expect(Listener.TrackChanges).toHaveBeenCalledWith(mapped.One(), 'Name', 'Titi');
     });

     it("should listen TrackChanges on nested object", function () {
         var Listener = { TrackChanges: function () { } };
         spyOn(Listener, 'TrackChanges');

         var mapped = ko.MapToObservable(basicmaped4, null, Listener);

         var newone = {Name:"Newman"};

         mapped.One(newone);

         expect(mapped.One()).toEqual(newone);
         expect(Listener.TrackChanges.calls.count()).toEqual(1);
         expect(Listener.TrackChanges).toHaveBeenCalledWith(mapped, 'One', newone);
     });


     it("should listen TrackCollectionChanges on collection", function () {
         var Listener = { TrackCollectionChanges: function (o, v, c) { console.log(o); console.log(v); console.log(c); } };
         spyOn(Listener, 'TrackCollectionChanges').and.callThrough();

         var mapped = ko.MapToObservable(basicmaped6, null, Listener);

         mapped.List().push({Name:"titi"});

         expect(mapped.List()().length).toEqual(4);
         expect(Listener.TrackCollectionChanges.calls.count()).toEqual(1);
     });


     it("should not listen TrackCollectionChanges on silent changes", function () {
         var Listener = { TrackCollectionChanges: function (o, v, c) { console.log(o); console.log(v); console.log(c); } };
         spyOn(Listener, 'TrackCollectionChanges').and.callThrough();

         var mapped = ko.MapToObservable(basicmaped6, null, Listener);

         mapped.List().silentremoveAll();

         expect(mapped.List()().length).toEqual(0);
         expect(Listener.TrackCollectionChanges.calls.count()).toEqual(0);
     });

});

describe("is Date function", function () {
    it("should not register TrackChanges on nested object", function () {
        var date = new Date(1, 1, 1);
        expect(ko.isDate(date)).toBe(true);
        expect(ko.isDate(null)).toBe(false);
        expect(ko.isDate({})).toBe(false);
        expect(ko.isDate(1)).toBe(false);
    });
    
});