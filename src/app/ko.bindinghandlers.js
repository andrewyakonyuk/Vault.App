/*global define, console */

define(["knockout"], function (ko) {
    'use strict';

    ko.bindingHandlers.pageTitle = {
        update: function (element, valueAccessor) {
            var title = valueAccessor();
            document.title = title;
        }
    };

    ko.virtualElements.allowedBindings.pageTitle = true;

    (function (undefined) {

        var componentLoadingOperationUniqueId = 0;

        ko.bindingHandlers['controller'] = {
            'init': function (element, valueAccessor, ignored1, ignored2, bindingContext) {
                var currentViewModel,
                    currentLoadingOperationId,
                    disposeAssociatedComponentViewModel = function () {
                        var currentViewModelDispose = currentViewModel && currentViewModel['dispose'];
                        if (typeof currentViewModelDispose === 'function') {
                            currentViewModelDispose.call(currentViewModel);
                        }

                        // Any in-flight loading operation is no longer relevant, so make sure we ignore its completion
                        currentLoadingOperationId = null;
                    };

                ko.utils.domNodeDisposal.addDisposeCallback(element, disposeAssociatedComponentViewModel);

                ko.computed(function () {
                    var value = ko.utils.unwrapObservable(valueAccessor()),
                        componentName, componentParams, afterRender;

                    if (typeof value === 'string') {
                        componentName = value;
                    } else {
                        componentName = ko.utils.unwrapObservable(value['name']);
                        componentParams = ko.utils.unwrapObservable(value['params']);
                        afterRender = ko.utils.unwrapObservable(value['afterRender'] || function () {
                            console.log('empty after render callback;');
                        });
                    }

                    if (!componentName) {
                        throw new Error('No component name specified');
                    }

                    var loadingOperationId = currentLoadingOperationId = ++componentLoadingOperationUniqueId;
                    ko.components.get(componentName, function (componentDefinition) {
                        // If this is not the current load operation for this element, ignore it.
                        if (currentLoadingOperationId !== loadingOperationId) {
                            return;
                        }

                        // Clean up previous state
                        disposeAssociatedComponentViewModel();

                        // Instantiate and bind new component. Implicitly this cleans any old DOM nodes.
                        if (!componentDefinition) {
                            throw new Error('Unknown component \'' + componentName + '\'');
                        }
                        cloneTemplateIntoElement(componentName, componentDefinition, element);
                        var componentViewModel = createViewModel(componentDefinition, element, componentParams),
                            childBindingContext = bindingContext['createChildContext'](componentViewModel);
                        currentViewModel = componentViewModel;
                        ko.applyBindingsToDescendants(childBindingContext, element);
                        afterRender(componentViewModel);
                        var viewModelAfterRender = componentViewModel['afterRender'];
                        if (typeof viewModelAfterRender === 'function') {
                            viewModelAfterRender.call(currentViewModel);
                        }
                    });
                }, null, {
                    disposeWhenNodeIsRemoved: element
                });

                return {
                    'controlsDescendantBindings': true
                };
            }
        };

        ko.virtualElements.allowedBindings['controller'] = true;

        function cloneTemplateIntoElement(componentName, componentDefinition, element) {
            var template = componentDefinition['template'];
            if (!template) {
                throw new Error('Component \'' + componentName + '\' has no template');
            }

            var clonedNodesArray = ko.utils.cloneNodes(template);
            ko.virtualElements.setDomNodeChildren(element, clonedNodesArray);
        }

        function createViewModel(componentDefinition, element, componentParams) {
            var componentViewModelFactory = componentDefinition['createViewModel'];
            return componentViewModelFactory ? componentViewModelFactory.call(componentDefinition, componentParams, {
                element: element
            }) : componentParams; // Template-only component
        }

    })();


});
