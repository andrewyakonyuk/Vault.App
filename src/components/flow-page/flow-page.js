define(['knockout', 'text!./flow-page.html', 'jquery'], function(ko, templateMarkup, $) {

  function FlowPage(params) {

  };


  // This runs when the component is torn down. Put here any logic necessary to clean up,
  // for example cancelling setTimeouts or disposing Knockout subscriptions/computeds.
  FlowPage.prototype.dispose = function() { };

  return { viewModel: FlowPage, template: templateMarkup };

});
