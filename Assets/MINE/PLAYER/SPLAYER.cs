using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class SPLAYER : MonoBehaviour
{
	private string[] COLORS;
	private Material[] MATS;
	private bool filledArrays = false;

	private string COLOR = "NONE";

	private Color bgColor = new Color(0.16f, 0.18f, 0.215f);

	private Coroutine lastPulsePlayerRoutine = null;
	private Coroutine lastPulseBGRoutine = null;
	private Coroutine lastStretchPlayerRoutine = null;
	private Coroutine lastTrailFollowRoutine = null;

	private Vector3 meshInitScale;

	public GameObject mesh;
	public GameObject outline;
	public GameObject trail;

	public GameObject deathVFX;
	public GameObject boomVFX;

	// Start is called before the first frame update
	void Start()
	{
		mesh.GetComponent<Renderer>().sharedMaterial.color = Color.black;

		Camera.main.backgroundColor = bgColor;

		meshInitScale = mesh.transform.localScale;

		trail.transform.position = transform.position;
		trail.GetComponent<TrailRenderer>().startColor = Color.white;

		deathVFX.GetComponent<VisualEffect>().Stop();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			SaveSystem.SaveData(SceneManager.GetActiveScene().name);
			SceneManager.LoadScene("MAIN_MENU");
		}
	}

	private float inputDelay = .04f;
	private float inputTimer = 0f;

	private float dirClearDelay = .06f;
	private float dirTimer = -1f;

	private Vector3 direction = Vector3.zero;
	private void FixedUpdate()
	{
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		if (input.x < 0)
		{
			direction = -transform.right;
			dirTimer = dirClearDelay;
		}
		else if (input.x > 0)
		{
			direction = transform.right;
			dirTimer = dirClearDelay;
		}
		else if (input.y < 0)
		{
			direction = -transform.forward;
			dirTimer = dirClearDelay;
		}
		else if (input.y > 0)
		{
			direction = transform.forward;
			dirTimer = dirClearDelay;
		}
		else
		{
			dirTimer = Mathf.Clamp(dirTimer - Time.fixedDeltaTime, -1f, dirClearDelay);
			if (dirTimer < 0f)
				direction = Vector3.zero;
		}

		inputTimer = Mathf.Clamp(inputTimer + Time.fixedDeltaTime, 0f, inputDelay + 1f);
		if (direction.Equals(Vector3.zero) == false && inputTimer > inputDelay)
		{
			RayCast(transform.position, direction, transform.position);
			inputTimer = 0f;
		}
	}

	private List<GameObject> lastHitButtons = new List<GameObject>();
	private void RayCast(in Vector3 castPos, in Vector3 dir, in Vector3 origPos)
	{
		LayerMask mask = (1 << 7) | (1 << 8);
		mask = ~mask;

		if (Physics.Raycast(castPos, dir, out RaycastHit hit, Mathf.Infinity, mask))
		{
			Vector3 old_pos, other_dir, new_scale;

			string tag = hit.transform.gameObject.tag;
			switch (tag)
			{
				case "BLOCK":
					if (Vector3.Distance(origPos + dir * .5f, hit.point) > .1f)
					{
						old_pos = origPos;
						transform.position = hit.point - dir * .5f;

						if (lastPulsePlayerRoutine != null)
							StopCoroutine(lastPulsePlayerRoutine);
						lastPulsePlayerRoutine = StartCoroutine(PulsePlayer(Color.white, 0f, 0f, .2f));

						if (lastStretchPlayerRoutine != null)
							StopCoroutine(lastStretchPlayerRoutine);
						other_dir = (AbsV3(dir) == transform.forward) ? transform.right : transform.forward;
						new_scale = MulV3(meshInitScale, AbsV3(dir)) * 2.0f
										+ MulV3(meshInitScale, AbsV3(other_dir)) * .5f
										+ MulV3(meshInitScale, !new Vector3Bool(AbsV3(dir) + AbsV3(other_dir)));
						lastStretchPlayerRoutine = StartCoroutine(StretchPlayer(meshInitScale, new_scale, 0f, 0f, .2f));

						if (lastTrailFollowRoutine != null)
						{
							trail.GetComponent<TrailRenderer>().Clear();
							trail.transform.position = old_pos;
							StopCoroutine(lastTrailFollowRoutine);
						}
						lastTrailFollowRoutine = StartCoroutine(TrailFollow(transform.position, .1f));
					}
					break;

				case "COLOR":
					var color_script = hit.transform.gameObject.GetComponent<SCOLOR>();
					if (filledArrays == false)
					{
						COLORS = color_script.COLORS;
						MATS = color_script.MATS;
						filledArrays = true;
					}

					ChangeColor(color_script.COLOR);
					mesh.GetComponent<Renderer>().sharedMaterial.color = Color2HSV(MATS[System.Array.IndexOf(COLORS, COLOR)].GetColor("_EmissionColor")).Saturate_Rel(.9f).Value_Rel(.9f).Hue_Rel(.95f).ToColor();
					trail.GetComponent<TrailRenderer>().startColor = mesh.GetComponent<Renderer>().sharedMaterial.color;

					if (lastPulseBGRoutine != null)
						StopCoroutine(lastPulseBGRoutine);
					lastPulseBGRoutine = StartCoroutine(PulseBG(Color2HSV(mesh.GetComponent<Renderer>().sharedMaterial.color).ToColor(), 0f, 0f, .75f));

					hit.transform.gameObject.layer = LayerMask.NameToLayer("IGNORE");
					Destroy(hit.transform.gameObject);
					//RayCast(origPos, dir, origPos);
					RayCast(hit.point, dir, origPos);

					break;

				case "BARRIER":
					var barrier_script = hit.transform.gameObject.GetComponent<SBARRIER>();
					if (filledArrays == false)
					{
						COLORS = barrier_script.COLORS;
						MATS = barrier_script.MATS;
						filledArrays = true;
					}

					if (barrier_script.COLOR != COLOR)
					{
						if(Vector3.Distance(origPos + dir * .5f, hit.point) > .1f)
						{
							barrier_script.PulseHit();

							old_pos = origPos;
							transform.position = hit.point - dir * .5f;

							if (lastPulsePlayerRoutine != null)
								StopCoroutine(lastPulsePlayerRoutine);
							lastPulsePlayerRoutine = StartCoroutine(PulsePlayer(Color.white, 0f, 0f, .2f));

							if (lastStretchPlayerRoutine != null)
								StopCoroutine(lastStretchPlayerRoutine);
							other_dir = (AbsV3(dir) == transform.forward) ? transform.right : transform.forward;
							new_scale = MulV3(meshInitScale, AbsV3(dir)) * 2.0f
											+ MulV3(meshInitScale, AbsV3(other_dir)) * .5f
											+ MulV3(meshInitScale, !new Vector3Bool(AbsV3(dir) + AbsV3(other_dir)));
							lastStretchPlayerRoutine = StartCoroutine(StretchPlayer(meshInitScale, new_scale, 0f, 0f, .2f));

							if (lastTrailFollowRoutine != null)
							{
								trail.GetComponent<TrailRenderer>().Clear();
								trail.transform.position = old_pos;
								StopCoroutine(lastTrailFollowRoutine);
							}
							lastTrailFollowRoutine = StartCoroutine(TrailFollow(transform.position, .1f));
						}
					}
					else
						RayCast(hit.point + dir * 1.1f, dir, origPos);

					break;

				case "SPIKE":
					inputDelay = 1000000f;
					StartCoroutine(RestartLevel(1.2f));
					StartCoroutine(SpawnParticles(0f, dir * 3f, dir * 7f));
					StartCoroutine(Explode(.15f));

					old_pos = origPos;
					transform.position = hit.point - dir * .5f;

					if (lastPulsePlayerRoutine != null)
						StopCoroutine(lastPulsePlayerRoutine);
					lastPulsePlayerRoutine = StartCoroutine(PulsePlayer(Color.white, 0f, 0f, .2f));

					if (lastStretchPlayerRoutine != null)
						StopCoroutine(lastStretchPlayerRoutine);
					other_dir = (AbsV3(dir) == transform.forward) ? transform.right : transform.forward;
					new_scale = MulV3(meshInitScale, AbsV3(dir)) * 2.0f
									+ MulV3(meshInitScale, AbsV3(other_dir)) * .5f
									+ MulV3(meshInitScale, !new Vector3Bool(AbsV3(dir) + AbsV3(other_dir)));
					lastStretchPlayerRoutine = StartCoroutine(StretchPlayer(meshInitScale, new_scale, 0f, 0f, .2f));

					if (lastTrailFollowRoutine != null)
					{
						trail.GetComponent<TrailRenderer>().Clear();
						trail.transform.position = old_pos;
						StopCoroutine(lastTrailFollowRoutine);
					}
					lastTrailFollowRoutine = StartCoroutine(TrailFollow(transform.position, .1f));

					break;

				case "BUTTON":
					var button_script = hit.transform.gameObject.GetComponent<SBUTTON>();

					if (dir.x < 0 && button_script.facing == SBUTTON.facing_options.LEFT
						|| dir.x > 0 && button_script.facing == SBUTTON.facing_options.RIGHT
						|| dir.z < 0 && button_script.facing == SBUTTON.facing_options.DOWN
						|| dir.z > 0 && button_script.facing == SBUTTON.facing_options.UP)
						button_script.Activate(dir);

					if (button_script.activated == false)
						lastHitButtons.Add(button_script.gameObject);
					hit.transform.gameObject.layer = LayerMask.NameToLayer("IGNORE");

					RayCast(origPos, dir, origPos);

					break;

				case "FINISH":
					inputDelay = 1000000f;
					var finish_script = hit.transform.gameObject.GetComponent<SFINISH>();
					finish_script.NextLevel(2f);
					//StartCoroutine(SpawnParticles(0f, dir * 3f, dir * 7f));
					StartCoroutine(TeleportEffect(.1f));

					old_pos = origPos;
					transform.position = hit.point - dir * .5f;

					if (lastPulsePlayerRoutine != null)
						StopCoroutine(lastPulsePlayerRoutine);
					lastPulsePlayerRoutine = StartCoroutine(PulsePlayer(Color.white, 0f, 0f, .2f));

					if (lastStretchPlayerRoutine != null)
						StopCoroutine(lastStretchPlayerRoutine);
					other_dir = (AbsV3(dir) == transform.forward) ? transform.right : transform.forward;
					new_scale = MulV3(meshInitScale, AbsV3(dir)) * 2.0f
									+ MulV3(meshInitScale, AbsV3(other_dir)) * .5f
									+ MulV3(meshInitScale, !new Vector3Bool(AbsV3(dir) + AbsV3(other_dir)));
					lastStretchPlayerRoutine = StartCoroutine(StretchPlayer(meshInitScale, new_scale, 0f, 0f, .2f));

					if (lastTrailFollowRoutine != null)
					{
						trail.GetComponent<TrailRenderer>().Clear();
						trail.transform.position = old_pos;
						StopCoroutine(lastTrailFollowRoutine);
					}
					lastTrailFollowRoutine = StartCoroutine(TrailFollow(transform.position, .1f));

					break;
			}
			if(lastHitButtons.Count > 0)
			{
				for (int i = 0; i < lastHitButtons.Count; ++i)
				{
					var button = lastHitButtons[i];
					if (Vector3.Distance(transform.position, button.transform.position) > .5f)
					{
						button.layer = LayerMask.NameToLayer("Default");
						lastHitButtons.Remove(button);
						--i;
					}
				}
			}
		}
	}

	private void ChangeColor(in string new_color)
	{
		if (COLOR == "RED" && new_color == "GREEN"
			|| COLOR == "GREEN" && new_color == "RED")
		{
			COLOR = "YELLOW";
		}
		else if (COLOR == "RED" && new_color == "YELLOW"
			|| COLOR == "YELLOW" && new_color == "RED")
		{
			COLOR = "ORANGE";
		}
		else if (COLOR == "BLUE" && new_color == "YELLOW"
			|| COLOR == "YELLOW" && new_color == "BLUE")
		{
			COLOR = "GREEN";
		}
		else if (COLOR == "RED" && new_color == "BLUE"
			|| COLOR == "BLUE" && new_color == "RED")
		{
			COLOR = "PURPLE";
		}
		else
		{
			COLOR = new_color;
		}
	}

	public struct HSV
	{
		public float h, s, v;

		public HSV Hue_Abs(in float new_hue)
		{
			h = new_hue;
			return this;
		}
		public HSV Hue_Rel(in float hue_coef)
		{
			h *= hue_coef;
			return this;
		}

		public HSV Saturate_Abs(in float new_sat)
		{
			s = new_sat;
			return this;
		}
		public HSV Saturate_Rel(in float sat_coef)
		{
			s *= sat_coef;
			return this;
		}

		public HSV Value_Abs(in float new_value)
		{
			v = new_value;
			return this;
		}
		public HSV Value_Rel(in float value_coef)
		{
			v *= value_coef;
			return this;
		}

		public Color ToColor()
		{
			return Color.HSVToRGB(h, s, v);
		}
	}

	public HSV Color2HSV(in Color color)
	{
		HSV hsv = new HSV();
		Color.RGBToHSV(color, out hsv.h, out hsv.s, out hsv.v);
		return hsv;
	}

	public Color HSV2Color(in HSV hsv)
	{
		return Color.HSVToRGB(hsv.h, hsv.s, hsv.v);
	}

	public IEnumerator PulseBG( Color new_color, float fade_in, float hold, float fade_out)
	{
		float fi_time = 0f, fo_time = 0f;

		while (fi_time < fade_in)
		{
			Camera.main.backgroundColor = Color.Lerp(bgColor, new_color, fi_time / fade_in);
			fi_time += Time.deltaTime;
			yield return null;
		}

		Camera.main.backgroundColor = new_color;
		if (hold > 0f)
			yield return new WaitForSeconds(hold);

		while (fo_time < fade_out)
		{
			Camera.main.backgroundColor = Color.Lerp(new_color, bgColor, fo_time / fade_out);
			fo_time += Time.deltaTime;
			yield return null;
		}
		Camera.main.backgroundColor = bgColor;
	}

	public IEnumerator PulsePlayer(Color new_color, float fade_in, float hold, float fade_out)
	{
		float fi_time = 0f, fo_time = 0f;

		Color old_color;

		if (filledArrays == false)
			old_color = Color.black;
		else
		{
			int mat_index = System.Array.IndexOf(COLORS, COLOR);
			if(mat_index < 0)
				old_color = Color.black;
			else
				old_color = Color2HSV(MATS[mat_index].GetColor("_EmissionColor")).Saturate_Rel(.9f).Value_Rel(.9f).Hue_Rel(.95f).ToColor();
		}

		while (fi_time < fade_in)
		{
			mesh.GetComponent<Renderer>().sharedMaterial.color = Color.Lerp(old_color, new_color, fi_time / fade_in);
			fi_time += Time.deltaTime;
			yield return null;
		}

		mesh.GetComponent<Renderer>().sharedMaterial.color = new_color;
		if (hold > 0f)
			yield return new WaitForSeconds(hold);

		while (fo_time < fade_out)
		{
			mesh.GetComponent<Renderer>().sharedMaterial.color = Color.Lerp(new_color, old_color, fo_time / fade_out);
			fo_time += Time.deltaTime;
			yield return null;
		}
		mesh.GetComponent<Renderer>().sharedMaterial.color = old_color;
	}

	public Vector3 MulV3(in Vector3 v0, in Vector3 v1)
	{
		return new Vector3(v0.x * v1.x, v0.y * v1.y, v0.z * v1.z);
	}

	public struct Vector3Bool
	{
		public bool x, y, z;

		public Vector3Bool(in bool _x, in bool _y, in bool _z)
		{
			x = _x;
			y = _y;
			z = _z;
		}
		public Vector3Bool(in Vector3 v)
		{
			x = (v.x != 0f) ? true : false;
			y = (v.y != 0f) ? true : false;
			z = (v.z != 0f) ? true : false;
		}

		public static implicit operator Vector3(Vector3Bool vb)
			=> new Vector3((vb.x) ? 1f : 0f, (vb.y) ? 1f : 0f, (vb.z) ? 1f : 0f);

		public static Vector3Bool operator !(Vector3Bool vb) => new Vector3Bool(!vb.x, !vb.y, !vb.z);

		public static Vector3 operator *(Vector3Bool vb, Vector3 v)
			=> new Vector3((vb.x) ? 1f : 0f * v.x, (vb.y) ? 1f : 0f * v.y, (vb.z) ? 1f : 0f * v.z);

		public static Vector3 operator *(Vector3 v, Vector3Bool vb)
			=> new Vector3((vb.x) ? 1f : 0f * v.x, (vb.y) ? 1f : 0f * v.y, (vb.z) ? 1f : 0f * v.z);
	}

	public Vector3 MulMaskedV3(in Vector3 v0, in Vector3 v1, in Vector3Bool mask)
	{
		return MulV3(MulV3(v0, mask), v1) + MulV3(v0, !mask);
	}

	public Vector3 AbsV3(in Vector3 v)
	{
		return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
	}

	public IEnumerator StretchPlayer(Vector3 old_scale, Vector3 new_scale, float fade_in, float hold, float fade_out)
	{
		float fi_time = 0f, fo_time = 0f;

		while (fi_time < fade_in)
		{
			mesh.transform.localScale = Vector3.Lerp(old_scale, new_scale, fi_time / fade_in);
			outline.transform.localScale = Vector3.Lerp(old_scale, new_scale, fi_time / fade_in);
			fi_time += Time.deltaTime;
			yield return null;
		}

		mesh.transform.localScale = new_scale;
		outline.transform.localScale = new_scale;
		if (hold > 0f)
			yield return new WaitForSeconds(hold);

		while (fo_time < fade_out)
		{
			mesh.transform.localScale = Vector3.Lerp(new_scale, old_scale, fo_time / fade_out);
			outline.transform.localScale = Vector3.Lerp(new_scale, old_scale, fo_time / fade_out);
			fo_time += Time.deltaTime;
			yield return null;
		}
		mesh.transform.localScale = old_scale;
		outline.transform.localScale = old_scale;
	}

	public IEnumerator TrailFollow(Vector3 pos, float delay)
	{
		Vector3 trailPos = trail.transform.position;
		float time = 0f;

		while (time < delay)
		{
			trail.transform.position = Vector3.Lerp(trailPos, pos, time / delay);
			time += Time.deltaTime;
			yield return null;
		}
		trail.transform.position = pos;
	}

	private IEnumerator RestartLevel(float delay)
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
	}

	private IEnumerator SpawnParticles(float delay, Vector3 min, Vector3 max)
	{
		yield return new WaitForSeconds(delay);

		if (filledArrays == false)
			deathVFX.GetComponent<VisualEffect>().SetVector4("COLOR", Color.black);
		else
			deathVFX.GetComponent<VisualEffect>().SetVector4("COLOR", MATS[System.Array.IndexOf(COLORS, COLOR)].GetColor("_EmissionColor"));

		deathVFX.GetComponent<VisualEffect>().SetVector3("MIN_DIST", min);
		deathVFX.GetComponent<VisualEffect>().SetVector3("MAX_DIST", max);

		mesh.GetComponent<Renderer>().enabled = false;
		outline.GetComponent<Renderer>().enabled = false;

		deathVFX.GetComponent<VisualEffect>().Play();
	}

	private IEnumerator Explode(float duration)
	{
		float timer = 0f;

		boomVFX.GetComponent<Renderer>().enabled = true;
		var mat = boomVFX.GetComponent<Renderer>().sharedMaterial;
		var power = mat.GetFloat("_POWER");
		var glow = mat.GetFloat("_GLOW");

		while (timer < duration)
		{
			boomVFX.transform.localScale += Vec3(7f) * Time.deltaTime;

			mat.SetFloat("_POWER", Mathf.Lerp(power, 4f, timer / duration));
			mat.SetFloat("_GLOW", Mathf.Lerp(glow, 0f, timer / duration));

			timer += Time.deltaTime;

			yield return null;
		}
		mat.SetFloat("_POWER", power);
		mat.SetFloat("_GLOW", glow);
		boomVFX.GetComponent<Renderer>().enabled = false;
	}

	private IEnumerator TeleportEffect(float delay)
	{
		yield return new WaitForSeconds(delay);

		float holdTime = .3f;
		var coroutine = StartCoroutine(StretchPlayer(meshInitScale, new Vector3(meshInitScale.x * 1.5f, meshInitScale.y, 100f), .2f, holdTime, 10f));
		StartCoroutine(PulsePlayer(Color.white, .2f, holdTime, 10f));
		yield return new WaitForSeconds(holdTime);

		StopCoroutine(coroutine);
		StartCoroutine(StretchPlayer(mesh.transform.localScale, new Vector3(0f, mesh.transform.localScale.y, mesh.transform.localScale.z), .5f, 10f, 10f));
		yield return new WaitForSeconds(.5f);

		mesh.GetComponent<Renderer>().enabled = false;
		outline.GetComponent<Renderer>().enabled = false;
	}

	public Vector3 Vec3(float nr)
	{
		return new Vector3(nr, nr, nr);
	}

	public Vector3 RandVec3()
	{
		return new Vector3(Random.value, Random.value, Random.value);
	}

	private void OnApplicationQuit()
	{
		mesh.GetComponent<Renderer>().sharedMaterial.color = Color.black;

		Camera.main.backgroundColor = bgColor;
	}
}
