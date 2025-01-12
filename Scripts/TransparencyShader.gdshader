shader_type spatial;

// Parameters that can be adjusted in the material
uniform float min_opacity : hint_range(0.0, 1.0) = 0.5;
uniform float max_opacity : hint_range(0.0, 1.0) = 1.0;
uniform float fade_speed : hint_range(0.1, 10.0) = 5.0;

uniform bool ray_is_colliding;

varying float transparency;

void vertex() {
    // Set transparency based on whether this object is occluding the character
    if (ray_is_colliding) {
        transparency = min_opacity;
    } else {
        transparency = max_opacity;
    }
}

void fragment() {
    // Apply base color and transparency
    ALBEDO = COLOR.rgb;
    ALPHA = transparency;
}