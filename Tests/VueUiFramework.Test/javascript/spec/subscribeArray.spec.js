/// <reference path="../src/subscribeArray.min.js" />


describe('Importing lib', function () {
    it('should add subscribe method to Array', function () {
        expect(!![].subscribe).toBe(true);
    });

    describe('when subscribe called', function () {
        it('should throw exception with wrong subject', function () {
            var wrong = function () {
                [].subscribe.call({});
            }
            expect(wrong).toThrow(new Error("subject should be an array"));
        });

        it('should throw exception with wrong function', function () {
            var wrong = function () {
                [].subscribe();
            }
            expect(wrong).toThrow(new Error("callBack should be a function"));
        });
    });

    function subscribe(arr, callBack) {
        return arr.subscribe(callBack);
    }

    describe('once listening', function () {
        var array, callback, pop, unsub;

        function initArray() {
            array = ['one', 'two'];
        }

        function createSubscription() {
            callback = jasmine.createSpy('callback');
            unsub = subscribe(array, callback);
            jasmine.clock().install();
        }

        function doBeforeAll() {
            initArray();
            createSubscription();
        }

        function doafterAll() {
            unsub();
            jasmine.clock().uninstall();
        }

        describe('after calling subscribe returned function', function () {
            beforeAll(function () {
                doBeforeAll();
                unsub();
                pop = array.pop();
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            })

            it('should not call subscribe', function () {
                expect(callback).not.toHaveBeenCalled();
            });

        });

        describe('after subscribing another function', function () {
            var callback2, unsub2;

            beforeAll(function () {
                doBeforeAll();
                callback2 = jasmine.createSpy('callback2');
                unsub2 = subscribe(array, callback2);
                pop = array.pop();
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
                unsub2();
            })

            it('should call the first listener', function () {
                expect(callback2).toHaveBeenCalled();
            });

            it('should call the second listener', function () {
                expect(callback2).toHaveBeenCalled();
            });

        });

        describe('if no changes happen', function () {

            beforeAll(function () {
                array = [1, 1];
                createSubscription();
            });

            afterAll(function () {
                doafterAll();
            })

            it('should not call listeners - sort', function () {
                array.sort();
                jasmine.clock().tick(0);
                expect(callback).not.toHaveBeenCalled();
            });

            it('should not call listeners - reverse', function () {
                array.reverse();
                jasmine.clock().tick(0);
                expect(callback).not.toHaveBeenCalled();
            });
        });

        describe('pop function', function () {
            var pop;

            beforeAll(function () {
                doBeforeAll();
                pop = array.pop();
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            });

            it('should work as original- return value', function () {
                expect(pop).toBe("two");
            });

            it('should work as original- return collection', function () {
                expect(array).toEqual(["one"]);
            });

            it('should send one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 1 element', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(1);
            });

            it('should send change with correct information: correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(1);
            });

            it('should send change with correct information: correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe(pop);
            });

            it('should send change with correct information: correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('deleted');
            });
        });

        describe('push function', function () {
            var added = "three", result;

            beforeAll(function () {
                doBeforeAll();
                result = array.push(added);
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            });

            it('should work as original- return value', function () {
                expect(result).toBe(3);
            });

            it('should work as original- return collection', function () {
                expect(array).toEqual(["one", "two", "three"]);
            });

            it('should send one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 1 element', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(1);
            });

            it('should send change with correct information: correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(2);
            });

            it('should send change with correct information: correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe(added);
            });

            it('should send change with correct information: correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('added');
            });
        });

        describe('shift function', function () {
            var result;

            beforeAll(function () {
                doBeforeAll();
                result = array.shift();
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            });

            it('should work as original- return value', function () {
                expect(result).toBe("one");
            });

            it('should work as original- return collection', function () {
                expect(array).toEqual(["two"]);
            });

            it('should send one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 1 element', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(1);
            });

            it('should send change with correct information: correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(0);
            });

            it('should send change with correct information: correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe("one");
            });

            it('should send change with correct information: correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('deleted');
            });
        });

        describe('unshift function', function () {
            var result, added1 = "newfirst", added2 = "newsecond";

            beforeAll(function () {
                doBeforeAll();
                result = array.unshift(added1, added2);
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            });

            it('should work as original- return value', function () {
                expect(result).toBe(4);
            });

            it('should work as original- return collection', function () {
                expect(array).toEqual([added1, added2, "one", "two"]);
            });

            it('should send one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 2 elements', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(2);
            });

            it('should send change with correct information: first change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(0);
            });

            it('should send change with correct information: first change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe(added1);
            });

            it('should send change with correct information: first change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('added');
            });

            it('should send change with correct information: second change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][1].index).toBe(1);
            });

            it('should send change with correct information: second change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][1].value).toBe(added2);
            });

            it('should send change with correct information: second change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][1].status).toBe('added');
            });
        });


        describe('splice function with 3 or more parameters and no remove', function () {
            var result, added1 = "newfirst", added2 = "newsecond";

            beforeAll(function () {
                doBeforeAll();
                result = array.splice(1, 0, added1, added2);
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            });

            it('should work as original- return value', function () {
                expect(result).toEqual([]);
            });

            it('should work as original- return collection', function () {
                expect(array).toEqual(["one", added1, added2, "two"]);
            });

            it('should send one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 2 elements', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(2);
            });

            it('should send change with correct information: first change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(1);
            });

            it('should send change with correct information: first change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe(added1);
            });

            it('should send change with correct information: first change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('added');
            });

            it('should send change with correct information: second change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][1].index).toBe(2);
            });

            it('should send change with correct information: second change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][1].value).toBe(added2);
            });

            it('should send change with correct information: second change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][1].status).toBe('added');
            });
        });

        describe('splice function with 0, 0 first parameters on empty array', function() {
            var result, added1 = "newfirst";          

            beforeAll(function() {
                doBeforeAll();
                array = [];
                result = array.splice(0, 0, added1);
                jasmine.clock().tick(0);
            });

            afterAll(function() {
                doafterAll();
            });

            it('should work as original- return value', function () {
                expect(result).toEqual([]);
            });

            it('should work as original- return collection', function () {
                expect(array).toEqual([added1]);
            });

            it('should send one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 1 element', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(1);
            });

            it('should send change with correct information: first change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(0);
            });

            it('should send change with correct information: first change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe(added1);
            });

            it('should send change with correct information: first change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('added');
            });
        });


        describe('splice function with 3 or more parameters and remove', function () {
            var result, added1 = "newfirst";

            beforeAll(function () {
                doBeforeAll();
                result = array.splice(1, 1, added1);
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            });

            it('should work as original- return value', function () {
                expect(result).toEqual(["two"]);
            });

            it('should work as original- collection', function () {
                expect(array).toEqual(["one", added1]);
            });

            it('should work as original- collection', function () {
                var arr = [1, 2, 3, 4];
                arr.subscribe(function () { });
                arr.splice(5, 1, 5);
                expect(arr).toEqual([1, 2, 3, 4, 5]);
            });

            it('should send one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 2 elements', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(2);
            });

            it('should send change with correct information: first change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(1);
            });

            it('should send change with correct information: first change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe("two");
            });

            it('should send change with correct information: first change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('deleted');
            });

            it('should send change with correct information: second change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][1].index).toBe(1);
            });

            it('should send change with correct information: second change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][1].value).toBe(added1);
            });

            it('should send change with correct information: second change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][1].status).toBe('added');
            });
        });


        describe('splice function with 2 parameters', function () {
            var result, added1 = "newfirst";

            beforeAll(function () {
                doBeforeAll();
                result = array.splice(0, 1);
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            });

            it('should work as original- return value', function () {
                expect(result).toEqual(["one"]);
            });

            it('should work as original- return collection', function () {
                expect(array).toEqual(["two"]);
            });

            it('should send one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 1 element', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(1);
            });

            it('should send change with correct information: first change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(0);
            });

            it('should send change with correct information: first change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe("one");
            });

            it('should send change with correct information: first change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('deleted');
            });
        });

        describe('splice function with 1 parameter', function () {
            var result;

            beforeAll(function () {
                doBeforeAll();
                result = array.splice(0);
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            });

            it('should work as original- return value', function () {
                expect(result).toEqual(['one', 'two']);
            });

            it('should work as original- return collection', function () {
                expect(array).toEqual([]);
            });

            it('should send one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 2 elements', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(2);
            });

            it('should send change with correct information: first change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(0);
            });

            it('should send change with correct information: first change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe('one');
            });

            it('should send change with correct information: first change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('deleted');
            });

            it('should send change with correct information: second change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][1].index).toBe(1);
            });

            it('should send change with correct information: second change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][1].value).toBe('two');
            });

            it('should send change with correct information: second change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][1].status).toBe('deleted');
            });
        });

        describe('splice function with 1 parameter: negative', function () {
            var result;

            beforeAll(function () {
                doBeforeAll();
                result = array.splice(-1);
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            });

            it('should work as original- return value', function () {
                expect(result).toEqual(['two']);
            });

            it('should work as original- return collection', function () {
                expect(array).toEqual(['one']);
            });

            it('should send one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 1 element', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(1);
            });

            it('should send change with correct information: first change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(1);
            });

            it('should send change with correct information: first change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe('two');
            });

            it('should send change with correct information: first change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('deleted');
            });
        });

        describe('reverse function', function () {
            var result;

            beforeAll(function () {
                doBeforeAll();
                result = array.reverse();
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            });

            it('should work as original- return value', function () {
                expect(result).toEqual(['two', 'one']);
            });

            it('should work as original- return collection', function () {
                expect(array).toEqual(['two', 'one']);
            });

            it('should send one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 4 elements', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(4);
            });

            it('should send change with correct information: first change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(0);
            });

            it('should send change with correct information: first change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe('one');
            });

            it('should send change with correct information: first change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('deleted');
            });

            it('should send change with correct information: first change : moved', function () {
                expect(callback.calls.mostRecent().args[0][0].moved).toBe(1);
            });

            it('should send change with correct information: 2nd change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][1].index).toBe(1);
            });

            it('should send change with correct information: 2nd change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][1].value).toBe('one');
            });

            it('should send change with correct information: 2nd change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][1].status).toBe('added');
            });

            it('should send change with correct information: 2nd change : moved', function () {
                expect(callback.calls.mostRecent().args[0][1].moved).toBe(0);
            });

            it('should send change with correct information: 3rd change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][2].index).toBe(1);
            });

            it('should send change with correct information: 3rd change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][2].value).toBe('two');
            });

            it('should send change with correct information: 3rd change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][2].status).toBe('deleted');
            });

            it('should send change with correct information: 3rd change : moved', function () {
                expect(callback.calls.mostRecent().args[0][2].moved).toBe(0);
            });

            it('should send change with correct information: 4th change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][3].index).toBe(0);
            });

            it('should send change with correct information: 4th change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][3].value).toBe('two');
            });

            it('should send change with correct information: 4th change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][3].status).toBe('added');
            });

            it('should send change with correct information: 4th change : moved', function () {
                expect(callback.calls.mostRecent().args[0][3].moved).toBe(1);
            });
        });


        describe('sort function', function () {
            var result;

            beforeAll(function () {
                array = [3, 2, 2, 1];
                createSubscription();
                result = array.sort();
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            });

            it('should work as original- return value', function () {
                expect(result).toEqual([1, 2, 2, 3]);
            });

            it('should work as original- return collection', function () {
                expect(array).toEqual([1, 2, 2, 3]);
            });

            it('should work as original- return collection- custo sort function', function () {
                var array = [1, 10, 100, 2, 20, 30, 300, 3000];
                array.subscribe(function () { });
                array.sort(function (a, b) {
                    return a - b;
                });
                expect(array).toEqual([1, 2, 10, 20, 30, 100, 300, 3000]);
            });

            it('should send one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 4 elements', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(4);
            });

            it('should send change with correct information: first change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(3);
            });

            it('should send change with correct information: first change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe(1);
            });

            it('should send change with correct information: first change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('deleted');
            });

            it('should send change with correct information: first change : moved', function () {
                expect(callback.calls.mostRecent().args[0][0].moved).toBe(0);
            });

            it('should send change with correct information: 2nd change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][1].index).toBe(0);
            });

            it('should send change with correct information: 2nd change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][1].value).toBe(1);
            });

            it('should send change with correct information: 2nd change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][1].status).toBe('added');
            });

            it('should send change with correct information: 2nd change : moved', function () {
                expect(callback.calls.mostRecent().args[0][1].moved).toBe(3);
            });

            it('should send change with correct information: 3rd change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][2].index).toBe(0);
            });

            it('should send change with correct information: 3rd change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][2].value).toBe(3);
            });

            it('should send change with correct information: 3rd change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][2].status).toBe('deleted');
            });

            it('should send change with correct information: 3rd change : moved', function () {
                expect(callback.calls.mostRecent().args[0][2].moved).toBe(3);
            });

            it('should send change with correct information: 4th change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][3].index).toBe(3);
            });

            it('should send change with correct information: 4th change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][3].value).toBe(3);
            });

            it('should send change with correct information: 4th change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][3].status).toBe('added');
            });

            it('should send change with correct information: 4th change : moved', function () {
                expect(callback.calls.mostRecent().args[0][3].moved).toBe(0);
            });
        });

        describe('append changes from diferent calls', function () {
            var result;

            beforeAll(function () {
                doBeforeAll();
                result = array.push("a");
                result = array.push("b");
                jasmine.clock().tick(0);
            });

            afterAll(function () {
                doafterAll();
            });

            it('should send only one change', function () {
                expect(callback.calls.count()).toBe(1);
            });

            it('should send change with correct information: 2 elements', function () {
                expect(callback.calls.mostRecent().args[0].length).toBe(2);
            });

            it('should send change with correct information: first change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][0].index).toBe(2);
            });

            it('should send change with correct information: first change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][0].value).toBe("a");
            });

            it('should send change with correct information: first change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][0].status).toBe('added');
            });

            it('should send change with correct information: 2nd change :correct index', function () {
                expect(callback.calls.mostRecent().args[0][1].index).toBe(3);
            });

            it('should send change with correct information: 2nd change : correct value', function () {
                expect(callback.calls.mostRecent().args[0][1].value).toBe("b");
            });

            it('should send change with correct information: 2nd change : correct status', function () {
                expect(callback.calls.mostRecent().args[0][1].status).toBe('added');
            });
        });
    });
});

