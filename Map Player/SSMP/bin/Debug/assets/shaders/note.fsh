#version 330

in vec4 pass_color;

out vec4 out_Color;

void main(void){
	out_Color = pass_color;
}