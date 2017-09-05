extends Node
var texSize = 64
var mapSize = Vector2(100,100)
var grassTexture = load("res://Sprites/rpgTile019.tex")

func _ready():
	for i in range(mapSize.x):
		for b in range(mapSize.y):
			var grass = Sprite.new()
			add_child(grass)
			grass.set_texture(grassTexture)
			grass.set_pos(Vector2(i * texSize,b * texSize))
			
	set_process(true)
	
func _process(delta):
	if(Input.is_mouse_button_pressed(BUTTON_LEFT)):
		var smallTree =load("res://Sprites/small_tree.tscn")
		var node = smallTree.instance()
		add_child(node)
		node.set_pos(get_viewport().get_mouse_pos() + get_child(0).get_pos())
		