define(["knockout", "text!./home.html"], function(ko, homeTemplate) {

  function HomeViewModel(route) {
      this.title = ko.observable("Home");
  }

  return { viewModel: HomeViewModel, template: homeTemplate };

});
