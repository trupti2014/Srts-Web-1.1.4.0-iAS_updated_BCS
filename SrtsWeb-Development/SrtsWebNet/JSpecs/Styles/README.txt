I followed SMACSS (Scalable and Modular Architecture for CSS), and BEM (Block Element Modifier) naming conventions.

Resource for http://smacss.com/
Resource for http://getbem.com/naming/

In order to compile the scss into css I used the CompileSass extension, which generates layouts.min.css and layouts.css.map

The Main.scss file contains a reference to all of the css files needed for the applications styling.

File Structure
Styles/
	Base/
		Base.scss
		Mixins.scss
		Normalize.css
		Variables.scss
	Layouts/
		Layouts.scss
	Modules/
		ConfirmOrder/
			ConfirmOrder.scss
		Elements/
			Elements.scss
		FAQ/
			FAQ.scss
		Loader/
			Loader.scss
		Login/
			Login.scss
		Modal/
			Modal.scss
		Navigation/
			Navigation.scss
		Orders/
			Orders.scss
		Profile/
			Profile.scss
		Title/
			Title.scss
	State/
	Theme/
Main.scss