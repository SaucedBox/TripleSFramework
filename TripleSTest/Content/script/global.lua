import ('TripleS', 'TripleS.Scripting')
--import ('MyGame', 'MyGame')

--Default global script.
--Use for entity based functions and common functions.
--Import your game assambely above.

local global = {}

function global.Foo()
	--Use your game's tools class here.
end

package.preload["global"] = function()
 	return global
end